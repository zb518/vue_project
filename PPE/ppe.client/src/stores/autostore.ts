import { get } from "@/http/axiosutils";
import router from "@/router";
import { defineStore } from "pinia";


export const useAuthStore = defineStore('auth', () => {

    /**
     * 保存token
     * @param token 
     */
    function saveToken(token: AccessTokenResponse) {
        sessionStorage.setItem("RefreshToken", token.RefreshToken)
        sessionStorage.setItem("AccessToken", token.AccessToken)
    }

    /**
     * 刷新token
     */
    function refreshToken() {
        const token = sessionStorage.getItem("RefreshToken")
        if (token) {
            sessionStorage.clear();
            router.push('/login')
        }
        else {
            get("/Account/Refresh", { RefreshToken: token }, (result: AccessTokenResponse) => {
                if (result) {
                    saveToken(result)
                }
            })
        }
    }


    return { refreshToken, saveToken }
})