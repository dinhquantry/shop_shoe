import { Variant, VariantCreateRequest } from "../types/variant";
import axiosClient from "./axiosClient";

export const VariantService = {
    getAll: (): Promise<Variant[]> => axiosClient.get('/variants'),
    getById: (id: number): Promise<Variant> => axiosClient.get(`/variants/${id}`),
    create: (data: VariantCreateRequest): Promise<Variant> => axiosClient.post('/variants', data),
    update: (id: number, data: VariantCreateRequest): Promise<void> => axiosClient.put(`/variants/${id}`, data),
    delete: (id: number): Promise<void> => axiosClient.delete(`/variants/${id}`)
};