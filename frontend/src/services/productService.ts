import axiosClient from "./axiosClient";
import { Product, ProductCreateRequest } from "../types/product";

export const ProductService = {
    getAll: (): Promise<Product[]> => {
        return axiosClient.get('/products');
    },

    create: (data: ProductCreateRequest): Promise<Product> => {
        return axiosClient.post('/products', data);
    },

    //  Hàm Cập Nhật (PUT)
    update: (id: number, data: ProductCreateRequest): Promise<void> => {
        return axiosClient.put(`/products/${id}`, data);
    },

    delete: (id: number): Promise<void> => {
        return axiosClient.delete(`/products/${id}`);
    },

    // 4. API TẢI ẢNH LÊN (Dùng FormData)
    uploadImage: (id: number, file: File, isMain: boolean): Promise<any> => {
        const formData = new FormData();
        formData.append("File", file); 
        formData.append("IsMain", String(isMain));

        return axiosClient.post(`/products/${id}/images`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data' // Bắt buộc khi gửi file
            }
        });
    }
};