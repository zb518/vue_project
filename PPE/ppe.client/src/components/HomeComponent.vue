<script setup lang="ts">
import * as yup from 'yup'
import { useField, useForm, ErrorMessage } from "vee-validate"
import InputRow from './forms/InputRow.vue'

const { meta, defineField } = useForm()

const [username, usernameAttrs] = defineField("username")
</script>

<template>
    <form method="post">
        <InputRow id="username" name="username" label="账号" />
    </form>
</template>

<style lang="scss" scoped></style>