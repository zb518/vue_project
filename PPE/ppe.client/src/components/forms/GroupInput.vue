<script setup lang="ts">
import { useField, ErrorMessage } from 'vee-validate'
const props = defineProps({
    id: String,
    name: String,
    type: String,
    label: String,
    placeholder: String,
})

const { value, errorMessage } = useField(() => props.name)
</script>

<template>
    <div class="input-group">
        <label class="input-group-text" for="{{ id }}">{{ label }}</label>
        <input class="form-control" v-model="value" :type="type || 'text'" :placeholder="placeholder || ''" :id="id"
            :name="name" />
    </div>
    <ErrorMessage class="text-danger" name="name" as="span" v-model="errorMessage" />
</template>

<style lang="scss" scoped></style>