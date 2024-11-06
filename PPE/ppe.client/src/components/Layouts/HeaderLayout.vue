<script setup lang="ts">
import LoginPartial from './LoginPartial.vue';


</script>

<template>
    <header>
        <nav class="navbar navbar-expand-lg bg-body-tertiary border-bottom mb-2">
            <div class="container-fluid">
                <a class="navbar-brand" href="/">PPE</a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse"
                    data-bs-target="#navbarSupportedContent" aria-controls="navbarSupportedContent"
                    aria-expanded="false" aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="collapse navbar-collapse" id="navbarSupportedContent">
                    <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                        <li class="nav-item">
                            <router-link class="nav-link" to="/">Home</router-link>
                        </li>
                        <li class="nav-item">
                            <router-link class="nav-link" to="/about">About</router-link>
                        </li>
                    </ul>
                    <LoginPartial />
                </div>
            </div>
        </nav>
    </header>
</template>

<style lang="scss" scoped>
.router-link-exact-active {
    color: #258cfb;
}
</style>