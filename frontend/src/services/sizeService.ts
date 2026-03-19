import axiosClient from "./axiosClient";
export const SizeService = {
    getAll: (): Promise<any[]> => axiosClient.get('/sizes')
};