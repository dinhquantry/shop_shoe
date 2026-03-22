import axiosClient from "./axiosClient";

export interface DanhMuc {
  id: number;
  tenDanhMuc: string;
  moTa: string | null;
  trangThai: boolean;
}

export interface DanhMucPayload {
  tenDanhMuc: string;
  moTa?: string | null;
  trangThai: boolean;
}

export interface Category {
  id: number;
  name: string;
  slug: string;
  isActive: boolean;
  parentId: number | null;
}

export interface CategoryPayload {
  name: string;
  slug?: string;
  isActive: boolean;
  parentId?: number | null;
}

export const fetchDanhMucs = () => {
  return axiosClient.get<DanhMuc[]>("/DanhMucs");
};

export const fetchDanhMucById = (id: number) => {
  return axiosClient.get<DanhMuc>(`/DanhMucs/${id}`);
};

export const createDanhMuc = (data: DanhMucPayload) => {
  return axiosClient.post<DanhMuc>("/DanhMucs", data);
};

export const updateDanhMuc = (id: number, data: DanhMucPayload) => {
  return axiosClient.put<DanhMuc>(`/DanhMucs/${id}`, data);
};

export const deleteDanhMuc = (id: number) => {
  return axiosClient.delete<void>(`/DanhMucs/${id}`);
};

export const categoryApi = {
  list: fetchDanhMucs,
  detail: fetchDanhMucById,
  create: createDanhMuc,
  update: updateDanhMuc,
  remove: deleteDanhMuc,
};

const mapDanhMucToCategory = (item: DanhMuc): Category => ({
  id: item.id,
  name: item.tenDanhMuc,
  slug: item.moTa ?? "",
  isActive: item.trangThai,
  parentId: null,
});

const mapCategoryPayloadToDanhMucPayload = (data: CategoryPayload): DanhMucPayload => ({
  tenDanhMuc: data.name,
  moTa: data.slug ?? null,
  trangThai: data.isActive,
});

export const fetchCategories = async (): Promise<{ data: Category[] }> => {
  const data = await fetchDanhMucs();
  return { data: data.map(mapDanhMucToCategory) };
};

export const createCategory = (data: CategoryPayload) => {
  return createDanhMuc(mapCategoryPayloadToDanhMucPayload(data));
};

export const updateCategory = (id: number, data: CategoryPayload) => {
  return updateDanhMuc(id, mapCategoryPayloadToDanhMucPayload(data));
};

export const deleteCategory = (id: number) => {
  return deleteDanhMuc(id);
};
