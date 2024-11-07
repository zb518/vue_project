<script setup lang="ts">
import { ref } from 'vue'
import $ from 'jquery'
import { post } from '@/http/axiosutils'
import * as yup from 'yup'
const imgUrl = ref('http://localhost:5085/api/Account/VerifyCode')
import { useForm } from 'vee-validate'


const { handleSubmit } = useForm({
    validationSchema: yup.object({
        username: yup.string().required("账号不能为空"),
        password: yup.string().required("密码不能为空"),
        verifycode: yup.string().required("验证码不能为空").length(4, "验证码为4个字符")
    })
})
const onSubmit = handleSubmit(values => {
    post("Account/Login", values, (rs: any) => {
        if (rs) {
            sessionStorage.setItem("Token", JSON.stringify(rs))
            // const authStore = useAuthStore()
            // authStore.saveSign(rs);
            // const query = router.currentRoute.value.query;
            // router.push(query?.redirect ? query.redirect as string : "/")
        }
    })
});

async function validateUserName(e) {
    const username: string = e.target.value;
    if (username.length > 0) {
        await post("/Account/ValidateUserName", { UserName: username }, (result: Boolean) => {
            if (!result) {
                e.target.focus();
                e.target.value = ''
                const parent: HTMLElement = e.target.parentElement.parentElement
                $(parent).find("span.text-danger").text("账号不正确")
            } else {
                e.target.removeEventListener("blur", validateUserName)
            }
        })
    }
}

async function validatePassword(userName: string) {
    await post("/Account/ValidatePassword", { UserName: userName }, (rs: Boolean) => {
        if (rs) {

        }
    })
}
</script>

<template>
    <form method="post" v-on:submit="onSubmit">
        <div class="mb-3">
            <GroupInput label="账号" placeholder="账号" name="username" id="username" />
        </div>
        <div class="mb-3">
            <GroupInput label="密码" placeholder="密码" name="password" id="password" type="password" />
        </div>
        <div class="mb-3 row">
            <div class="col-7">
                <GroupInput label="验证码" placeholder="验证码" name="verifycode" id="verifycode" />
            </div>
            <div class="col-5">
                <img :src="imgUrl" alt="" onclick="this.src=this.src+'?'+Math.random()">
            </div>
        </div>
        <div class="mb-3">
            <button type="submit" class="btn btn-success w-100">登录</button>
        </div>
    </form>
</template>

<style lang="scss" scoped></style>