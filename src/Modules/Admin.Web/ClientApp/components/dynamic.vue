<template>
    <div>
        <v-runtime-template :template="template"></v-runtime-template>
    </div>
</template>

<script>
    import vRuntimeTemplate from "v-runtime-template";

    export default {
        data: () => ({
            name: "Mellow",
            template: 'Loading...'
        }),
        props: [
            "component",
        ],
        watch: {
            component: function (o, n) {
                this.loadPage();
            },
        },
        methods: {
            async loadPage() {
                try {
                    var data = await this.$http.get("/Uptime.Plugin/components/servers.html");
                    this.template = data.data; 
                } catch (err) {
                    alert(err);
                } 
            },
        },
        components: {
            vRuntimeTemplate
        },
        mounted: async function () {
            this.loadPage();
        },
    };
</script>
