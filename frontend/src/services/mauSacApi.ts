import axiosClient from "./axiosClient";

export interface MauSac {
  id: number;
  tenMau: string;
  maHex: string | null;
}

export interface MauSacPayload {
  tenMau: string;
  maHex?: string | null;
}

export const fetchMauSacs = () => {
  return axiosClient.get<MauSac[]>("/MauSacs");
};

export const fetchMauSacById = (id: number) => {
  return axiosClient.get<MauSac>(`/MauSacs/${id}`);
};

export const createMauSac = (data: MauSacPayload) => {
  return axiosClient.post<MauSac>("/MauSacs", data);
};

export const updateMauSac = (id: number, data: MauSacPayload) => {
  return axiosClient.put<MauSac>(`/MauSacs/${id}`, data);
};

export const deleteMauSac = (id: number) => {
  return axiosClient.delete<void>(`/MauSacs/${id}`);
};

export const mauSacApi = {
  list: fetchMauSacs,
  detail: fetchMauSacById,
  create: createMauSac,
  update: updateMauSac,
  remove: deleteMauSac,
};
