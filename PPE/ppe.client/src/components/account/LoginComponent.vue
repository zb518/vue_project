<script setup lang="ts">
import { post, get } from "@/http/axiosutils";
import { Form, Field, ErrorMessage } from "vee-validate"
import * as yup from "yup";
import { useAuthStore } from "@/stores/autostore";

const validateUserName = yup.string().required("账号不能为空").min(5, "账号不能少于5个字符")
const validatePassword = yup.string().required("密码不能为空").min(5, "密码不能少于5个字符")
const validateVerifyCode = yup.string().required("验证码不能为空").length(4, "验证码长度必须4个字符")

function onSubmit(values) {
    post("/Account/Login", values, (token: AccessTokenResponse) => {
        if (token.accessToken) {
            var authStore = useAuthStore()
            authStore.saveToken(token)
            get("/Account/SignInUser", null, (user: SignInUser) => {
                authStore.saveUser(user);
            })
        } else {
            alert(token.detail);
            document.querySelectorAll("input").forEach((e) => e.value = '');
            document.getElementById("username")?.focus()
        }
    })
}
</script>

<template>
    <Form @submit="onSubmit">
        <div class="mb-3">
            <label for="username">账号</label>
            <Field name="username" type="text" class="form-control" :rules="validateUserName" />
            <ErrorMessage name="username" class="text-danger" />
        </div>
        <div class="mb-3">
            <label for="password">密码</label>
            <Field name="password" type="password" class="form-control" :rules="validatePassword" />
            <ErrorMessage name="password" class="text-danger" />
        </div>
        <div class="row mb-3">
            <div class="col-7">
                <label for="verifyCode">验证码</label>
                <Field name="verifyCode" type="verifyCode" class="form-control" :rules="validateVerifyCode" />
                <ErrorMessage name="verifyCode" class="text-danger" />
            </div>
            <div class="col-5">
                <img src="http://localhost:5085/api/Account/VerifyCode" alt=""
                    onclick="this.src=this.src+'?'+Math.random()">
            </div>
        </div>
        <div class="mb-3">
            <button type="submit" class="btn btn-success w-100">Log In</button>
        </div>
    </Form>
</template>

<style lang="scss" scoped></style>