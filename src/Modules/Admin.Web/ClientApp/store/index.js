import Vue from 'vue'
import Vuex from 'vuex'
import tsummary from './modules/ticketSummary'
import itemsppage from './modules/itemsPerPage'

Vue.use(Vuex)

export default new Vuex.Store({
    modules: {
        tsummary,
        itemsppage,
    },
})
