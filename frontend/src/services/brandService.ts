import axiosClient from "./axiosClient";
import { Brand, BrandRequest } from "../types/brand";

export const BrandService = {
    getAll: (): Promise<Brand[]> => axiosClient.get('/thuonghieus'),
    getById: (id: number): Promise<Brand> => axiosClient.get(`/thuonghieus/${id}`),
    create: (data: BrandRequest): Promise<Brand> => axiosClient.post('/thuonghieus', data),
    update: (id: number, data: BrandRequest): Promise<Brand> => axiosClient.put(`/thuonghieus/${id}`, data),
    delete: (id: number): Promise<void> => axiosClient.delete(`/thuonghieus/${id}`)
};