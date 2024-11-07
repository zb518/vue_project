<script setup lang="ts">
import { useField, ErrorMessage } from 'vee-validate'
import { required } from '@vee-validate/rules'
import { localize } from '@vee-validate/i18n'

const props = defineProps({
    id: String,
    name: String,
    type: String,
    label: String,
})

const { value, errorMessage } = useField(() => props.name)
</script>

<template>
    <label for="{{ id }}">{{ label }}</label>
    <input class="form-control" v-model="value" :type="type || 'text'" :id="id" :name="name" />
    <span class="text-danger">{{ errorMessage }}</span>
</template>

<style lang="scss" scoped></style>