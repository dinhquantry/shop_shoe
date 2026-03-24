import axiosClient from "./axiosClient";
import { Category, CategoryRequest } from "../types/category";
export const CategoryService = {
    getAll: function (): Promise<Category[]> {
        return axiosClient.get('/danhmucs');
    },
    getById: function (id: number): Promise<Category> {
        return axiosClient.get(`/danhmucs/${id}`);
    },
    create: (data: CategoryRequest): Promise<Category> => axiosClient.post('/danhmucs', data),
    update: (id: number, data: CategoryRequest): Promise<Category> => axiosClient.put(`/danhmucs/${id}`, data),
    delete: (id: number): Promise<void> => axiosClient.delete(`/danhmucs/${id}`)
};