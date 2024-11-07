import { useAuthStore } from '@/stores/autostore';
import axios from 'axios'
import { ref } from 'vue';
const baseURL = ref('http://localhost:5085/api')

const service = axios.create({
    baseURL: "http://localhost:5085/api",
    timeout: 5000,
});

/**
 * 请求拦截
 */
service.interceptors.request.use(
    config => {
        const token = sessionStorage.getItem("AccessToken");
        if (token) {
            config.headers["Authorization"] = `Bearer ${token}`;
        }
        return config;
    },
    error => {
        return Promise.reject(error)
    }
)


/**
 * 响应拦截
 */
service.interceptors.response.use(
    config => {
        return config;
    },
    error => {
        if (error.response.status) {
            switch (error.response.status) {
                case 400:
                    error.message = "错误请求";
                    break;
                case 401:
                    error.message = "未授权，请重新登录";
                    const authStore = useAuthStore()
                    authStore.refreshToken()
                    break;
                case 403:
                    error.message = "拒绝访问";
                    break;
                case 404:
                    error.message = "请求错误，未找到该资源！！！";
                    break;
                case 405:
                    error.message = "请求方法未允许";
                    break;
                case 408:
                    error.message = "请求超时";
                    break;
                case 500:
                    error.message = "服务器端出错";
                    break;
                case 501:
                    error.message = "网络未实现";
                    break;
                case 502:
                    error.message = "网络错误";
                    break;
                case 503:
                    error.message = "服务不可用";
                    break;
                case 504:
                    error.message = "网络超时";
                    break;
                case 505:
                    error.message = "http版本不支持该请求";
                    break;
                default:
                    error.message = `连接错误${error.response.status}`;
                    break;
            }
        }
    }
)

/**
 * 
 */
export default { service, baseURL }

/**
 * 
 * @param url 
 * @param params 
 * @param backcall 
 */
export function get(url: string, params: object, backcall: Function) {
    service.get(url, { params: params })
        .then(res => {
            backcall(res.data)
        }).catch(err => {
            console.error(err)
        })
}

/**
 * 
 * @param url 
 * @param params 
 * @param backcall 
 */
export function post(url: string, params: object, backcall: Function) {
    service.post(url, params)
        .then(res => {
            backcall(res.data)
        }).catch(err => {
            console.error(err)
        })
}