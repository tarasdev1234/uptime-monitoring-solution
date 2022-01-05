<template>
    <select class="form-control m-bootstrap-select">
        <option value="1">1</option>
        <option value="10">10</option>
        <option value="20">20</option>
        <option value="30">30</option>
        <option value="50">50</option>
        <option value="100">100</option>
    </select>
</template>

<script>
    import { mapGetters } from "vuex"

    export default {
        computed: mapGetters({
            perPage: "itemsPerPage"
        }),
        props: ['value'],
        mounted: function () {
            var vm = this;
            this.value = this.perPage;

            $(this.$el).selectpicker()
                .val(this.value)
                .trigger('change')
                .on('change', function () {
                    vm.$emit('input', this.value);
                    vm.$store.commit("setItemsPerPage", this.value);
                });

            $(this.$el).selectpicker("refresh");
        },
        watch: {
            value: function (value) {
                $(this.$el)
                    .val(value)
                    .trigger('change')
            }
        },
    }
</script>
