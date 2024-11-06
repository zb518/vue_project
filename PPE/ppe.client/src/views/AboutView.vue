<script setup lang="ts">
import { onMounted } from 'vue';
import $ from 'jquery'
onMounted(() => {
  const t = $('h1').addClass("text-primary").text()
  console.log(t)
})
</script>

<template>
  <div class="card">
    <div class="card-header">
      <h5 class="card-title">About</h5>
    </div>
    <div class="card-body">
      <h1>This is an about page</h1>
    </div>
  </div>
</template>


<style lang="scss" scoped></style>
