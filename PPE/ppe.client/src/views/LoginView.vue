<script setup lang="ts">
import LoginComponent from '@/components/account/LoginComponent.vue';


</script>

<template>
    <div class="login-box">
        <div class="card">
            <div class="card-header text-center">
                <h1 class="card-title">Log in</h1>
            </div>
            <div class="card-body">
                <LoginComponent />
                <div class="mb-3">
                    <router-link to="/" class="btn btn-primary w-100">
                        <font-awesome-icon icon="fa-solid fa-home"></font-awesome-icon>主页</router-link>
                </div>
            </div>
        </div>
    </div>
</template>

<style lang="scss" scoped>
.login-box {
    position: absolute;
    display: block;
    left: 50%;
    top: 40%;
    transform: translate(-50%, -50%);
    min-width: 400px;
}
</style>