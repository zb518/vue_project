import { get } from "@/http/axiosutils";
import router from "@/router";
import { defineStore } from "pinia";


export const useAuthStore = defineStore('auth', () => {

    /**
     * 保存token
     * @param token 
     */
    function saveToken(token: AccessTokenResponse) {
        // sessionStorage.setItem("RefreshToken", token.refreshToken)
        // sessionStorage.setItem("AccessToken", token.accessToken)
        sessionStorage.setItem("Token", JSON.stringify(token, null, 2))
    }

    /**
     * 刷新token
     */
    function refreshToken() {
        const tokenJson = sessionStorage.getItem("Token")
        if (tokenJson) {
            const token = JSON.parse(tokenJson) as AccessTokenResponse
            if (token?.accessToken?.length > 0) {
                sessionStorage.clear();
                router.push('/login')
            }
            else {
                get("/Account/Refresh", { RefreshToken: token.refreshToken }, (result: AccessTokenResponse) => {
                    if (result) {
                        saveToken(result)
                    }
                })
            }
        } else {
            sessionStorage.clear();
            router.push('/login')
        }
    }


    function saveUser(user: SignInUser) {
        sessionStorage.setItem("AuthUser", JSON.stringify(user, null, 2))
    }

    return { refreshToken, saveToken, saveUser }
})