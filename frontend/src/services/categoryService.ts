import axiosClient from "./axiosClient";
import { Category, CategoryCreateRequest } from "../types/category";

export const CategoryService = {
    getAll: (): Promise<Category[]> => {
        return axiosClient.get('/categories');
    },
    create: (data: CategoryCreateRequest): Promise<Category> => {
        return axiosClient.post('/categories', data);
    },
    delete: (id: number): Promise<void> => {
        return axiosClient.delete(`/categories/${id}`);
    },
    update: (id: number, data: CategoryCreateRequest): Promise<void> => {
        return axiosClient.put(`/categories/${id}`, data);
    }
};