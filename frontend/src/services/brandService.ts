// File: services/brandService.ts
import axiosClient from "./axiosClient";
import { Brand, BrandCreateRequest } from "../types/brand";

export const BrandService = {
    getAll: (): Promise<Brand[]> => {
        return axiosClient.get('/brands');
    },
    
    create: (data: BrandCreateRequest): Promise<Brand> => {
        return axiosClient.post('/brands', data);
    },
    
    update: (id: number, data: BrandCreateRequest): Promise<void> => {
        return axiosClient.put(`/brands/${id}`, data);
    },
    
    delete: (id: number): Promise<void> => {
        return axiosClient.delete(`/brands/${id}`);
    },

    // API Up Logo riêng biệt (Giống hệt C# yêu cầu)
    uploadLogo: (id: number, file: File): Promise<any> => {
        const formData = new FormData();
        formData.append("file", file); // Chữ "file" phải khớp với tham số IFormFile trong C#

        return axiosClient.post(`/brands/${id}/logo`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
    }
};