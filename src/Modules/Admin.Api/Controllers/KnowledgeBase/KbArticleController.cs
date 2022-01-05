using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Admin.Api.ViewModels;
using Admin.Api.ViewModels.Kb;
using Uptime.Data;
using Uptime.Data.Models.KnowledgeBase;
using Admin.Api.Constants;
using Uptime.Mvc.Controllers;
using Helpers;
using Reliablesite.Service.Model.Dto;

namespace KnowledgeBaseApi.Controllers
{
    [Route("api/kb")]
    [Authorize(Roles = "Admin")]
    public class KbArticleController : BaseController {
        public KbArticleController (ApplicationDbContext context) : base(context) {}

        [Authorize(Policy = Permissions.READ_ARTICLES)]
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<KbArticle>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPosts ([FromQuery] PagedQuery p, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = dbContext.KbArticles.AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(pg => pg.Title.Contains(search));
            }

            var count = await query.LongCountAsync();
            var posts = await query
                        .Include(a => a.Comments)
                        .Include(a => a.Author)
                        .Include(a => a.KbCategory)
                        .Include(a => a.PostTags)
                        .Select(a => new KbArticle() {
                            Id = a.Id,
                            AuthorId = a.AuthorId,
                            Title = a.Title,
                            DateCreated = a.DateCreated,
                            Excerpt = a.Excerpt,
                            Slug = a.Slug,
                            Content = a.Content,
                            SeriesId = a.SeriesId,
                            SeriesOrder = a.SeriesOrder,
                            KbCategoryId = a.KbCategoryId,
                            TagNames = a.PostTags.Select(pt => pt.Tag.Name).ToList(),
                            KbCategoryName = a.KbCategory.Name,
                            AuthorName = a.Author.UserName,
                            CommentsCount = a.Comments.Count,
                        })
                        .OrderBy(a => a.Id)
                        .Paged(p)
                        .ToListAsync();

            var view = new PaginatedItemsVm<KbArticle>(p.PageIndex.Value, p.PageSize.Value, count, posts);

            return Ok(view);
        }

        [Authorize(Policy = Permissions.READ_ARTICLES)]
        [HttpGet("{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPost (long id) {
            if (id <= 0) {
                return BadRequest();
            }

            var post = await dbContext.KbArticles
                            .Include(a => a.PostBrands)
                            .Include(a => a.PostTags)
                            .Where(a => a.Id == id)
                            .Select(a => new KbArticle() {
                                Id = a.Id,
                                Title = a.Title,
                                Slug = a.Slug,
                                KbCategoryId = a.KbCategoryId,
                                IsPublished = a.IsPublished,
                                AuthorId = a.AuthorId,
                                Content = a.Content,
                                ShowInAll = a.ShowInAll,
                                Brands = a.PostBrands.Select(b => b.BrandId).ToList(),
                                Tags = a.PostTags.Select(pt => pt.TagId).ToList(),
                                DateCreated = a.DateCreated
                            })
                            .FirstOrDefaultAsync();

            if (post == null) {
                return NotFound();
            }

            return Ok(post);
        }
        
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreatePost ([FromBody]KbArticleViewModel value) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            
            var post = new KbArticle() {
                Title = value.Title,
                ShowInAll = value.ShowInAllBrands,
                Content = value.Content,
                KbCategoryId = value.KbCategoryId,
                IsPublished = value.IsPublished,
                Slug = Hash.Md5(value.Title.Replace(' ', '_').ToLower() + new Random().Next(999, 999999)),
                AuthorId = value.AuthorId,
                DateCreated = DateTime.Now
            };

            post.PostBrands = new List<KbArticleBrand>();

            if (value.Brands != null) {
                foreach (var brndId in value.Brands) {
                    post.PostBrands.Add(new KbArticleBrand() {
                        Article = post,
                        BrandId = brndId
                    });
                }
            }

            dbContext.KbArticles.Add(post);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, null);
        }

        [Authorize(Policy = Permissions.DELETE_ARTICLES)]
        [Route("{id:long}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteArticle (string id) {
            var post = await dbContext.KbArticles.FindAsync(id);

            if (post == null) {
                return NotFound();
            }

            dbContext.KbArticles.Remove(post);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Update ([FromBody]KbArticle postToUpdate) {
            var art = await dbContext.KbArticles.AnyAsync(a => a.Id == postToUpdate.Id);

            if (!art) {
                return NotFound(new { Message = $"Post with id {postToUpdate.Id} not found." });
            }
            
            if (postToUpdate.Slug == null) {
                postToUpdate.Slug = Hash.Md5(postToUpdate.Title.Replace(' ', '_').ToLower() + new Random().Next(999, 999999));
            }

            var currentBrnds = await dbContext.KbArticleBrands
                                              .Where(ab => ab.ArticleId == postToUpdate.Id)
                                              .ToListAsync();
            
            var delBrnds = currentBrnds.Where(pb => !postToUpdate.Brands.Any(nb => nb == pb.BrandId)).ToList();
            var addBrnds = postToUpdate.Brands.Where(nb => !currentBrnds.Any(pb => pb.BrandId == nb)).ToList();

            if (delBrnds.Count > 0) {
                dbContext.KbArticleBrands.RemoveRange(delBrnds);
            }

            if (addBrnds.Count > 0) {
                addBrnds.ForEach(id => dbContext.KbArticleBrands.Add(new KbArticleBrand() {
                    BrandId = id,
                    ArticleId = postToUpdate.Id
                }));
            }

            var tagsToRemove = await dbContext.KbArticleTags.Where(at => at.ArticleId == postToUpdate.Id).ToListAsync();

            if (tagsToRemove.Count > 0) {
                dbContext.KbArticleTags.RemoveRange(tagsToRemove);
            }

            if (postToUpdate.Tags.Count > 0) {
                postToUpdate.Tags.ForEach(id => dbContext.KbArticleTags.Add(new KbArticleTag() {
                    TagId = id,
                    ArticleId = postToUpdate.Id
                }));
            }

            dbContext.Update(postToUpdate);

            await dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPost), new { id = postToUpdate.Id }, null);
        }

        [HttpGet("{id:long}/comments")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(PaginatedItemsVm<KbComment>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetComments (long id, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0) {
            if (id <= 0) {
                return BadRequest();
            }

            var post = await dbContext
                        .KbArticles
                        .Include(p => p.Comments)
                        .Where(p => p.Id == id)
                        .FirstOrDefaultAsync();

            if (post == null) {
                return NotFound();
            }

            var count = post.Comments.LongCount();
            var comments = post.Comments
                        .OrderBy(p => p.Id)
                        .Skip(pageSize * pageIndex)
                        .Take(pageSize)
                        .ToList();

            var view = new PaginatedItemsVm<KbComment>(pageIndex, pageSize, count, comments);

            return Ok(post);
        }
    }
}
