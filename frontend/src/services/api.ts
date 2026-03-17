import axiosClient from "../lib/axiosClient";

// Định nghĩa cấu trúc cục dữ liệu sẽ gửi đi (bỏ Id vì Backend tự tăng)
interface CategoryPayload {
  name: string;
  slug: string;
  isActive: boolean;
  parentId: number | null;
}

// 1. Lấy danh sách (Read)
export const fetchCategories = () => {
  return axiosClient.get('/categories'); 
};

// 2. Thêm mới (Create)
export const createCategory = (data: CategoryPayload) => {
  return axiosClient.post('/categories', data);
};

// 3. Cập nhật (Update)
export const updateCategory = (id: number, data: CategoryPayload) => {
  return axiosClient.put(`/categories/${id}`, data);
};

// 4. Xóa (Delete)
export const deleteCategory = (id: number) => {
  return axiosClient.delete(`/categories/${id}`);
};