import axios from "axios";

const axiosClient=axios.create({
    baseURL:"http://localhost:5266/api",
    headers:{
        'Content-Type':'application/json',
    },
});
axiosClient.interceptors.response.use(
    (respone)=>respone.data,
    (error)=>{
        console.error("Lỗi gọi api:" ,error);
        return Promise.reject(error);
    }
);
export default axiosClient;