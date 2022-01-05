import TicketsApi from "../../Services/tickets"

const state = {
    ticketSummary: {
        "queue": 0,
        "active": 0,
        "waiting": 0,
        "activeGlobal": 0,
        "waitingGlobal": 0
    },
}

const mutations = {
    setSummary(state, summary) {
        state.ticketSummary = summary
    },
}

const getters = {
    ticketSummary: state => state.ticketSummary,
}

const actions = {
    updateSummary({ commit, state }) {
        TicketsApi.getSummary(
            (data) => commit("setSummary", data),
            (err) => alert("Ticket summary update err: " + err)
        )
    },
}

export default {
    state,
    getters,
    actions,
    mutations
}
