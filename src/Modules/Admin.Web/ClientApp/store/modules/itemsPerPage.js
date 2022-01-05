const state = {
    itemsPerPage: 10
}

const mutations = {
    setItemsPerPage(state, value) {
        state.itemsPerPage = value
    },
}

const getters = {
    itemsPerPage: state => state.itemsPerPage,
}

const actions = {
}

export default {
    state,
    getters,
    actions,
    mutations
}
