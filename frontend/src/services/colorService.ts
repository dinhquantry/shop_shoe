import axiosClient from "./axiosClient";
export const ColorService = {
    getAll: (): Promise<any[]> => axiosClient.get('/colors')
};