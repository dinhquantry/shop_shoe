import axiosClient from "./axiosClient";

export interface Size {
  id: number;
  tenSize: string;
}

export interface SizePayload {
  tenSize: string;
}

export const fetchSizes = () => {
  return axiosClient.get<Size[]>("/Sizes");
};

export const fetchSizeById = (id: number) => {
  return axiosClient.get<Size>(`/Sizes/${id}`);
};

export const createSize = (data: SizePayload) => {
  return axiosClient.post<Size>("/Sizes", data);
};

export const updateSize = (id: number, data: SizePayload) => {
  return axiosClient.put<Size>(`/Sizes/${id}`, data);
};

export const deleteSize = (id: number) => {
  return axiosClient.delete<void>(`/Sizes/${id}`);
};

export const sizeApi = {
  list: fetchSizes,
  detail: fetchSizeById,
  create: createSize,
  update: updateSize,
  remove: deleteSize,
};
