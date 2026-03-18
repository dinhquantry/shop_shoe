import axiosClient from "../lib/axiosClient";

export interface CategoryPayload {
  name: string;
  description?: string | null;
}

export interface CategoryDto {
  id: number;
  name: string;
  description?: string | null;
}

// 1. Lấy danh sách (Read)
export const fetchCategories = () => {
  return axiosClient.get<CategoryDto[]>('/categories'); 
};

// 2. Thêm mới (Create)
export const createCategory = (data: CategoryPayload) => {
  return axiosClient.post<CategoryDto>('/categories', data);
};

// 3. Cập nhật (Update)
export const updateCategory = (id: number, data: CategoryPayload) => {
  return axiosClient.put(`/categories/${id}`, data);
};

// 4. Xóa (Delete)
export const deleteCategory = (id: number) => {
  return axiosClient.delete(`/categories/${id}`);
};
