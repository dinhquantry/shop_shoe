"use client";

import React, { useState, useEffect } from "react";
import { Plus, Edit, Trash2, X, Save } from "lucide-react";
import clsx from "clsx";
import { createCategory, deleteCategory, fetchCategories, updateCategory } from "@/src/services/api";
// Import các hàm gọi API thật (Đảm bảo bạn đã tạo file này ở Bước 3)

// Định nghĩa model giống với C#
interface Category {
  id: number;
  name: string;
  slug: string;
  isActive: boolean;
  parentId?: number | null;
  children?: Category[]; 
}

export default function CategoryPage() {
  // 1. STATE QUẢN LÝ DANH SÁCH
  const [categories, setCategories] = useState<Category[]>([]);
  
  // 2. STATE QUẢN LÝ FORM & MODAL
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editCategoryId, setEditCategoryId] = useState<number | null>(null);

  // State hứng dữ liệu từ các ô Input
  const [name, setName] = useState("");
  const [slug, setSlug] = useState("");
  const [isActive, setIsActive] = useState(true);
  const [parentId, setParentId] = useState("");

  // ==========================================================
  // LẤY DỮ LIỆU TỪ BACKEND C# (Read)
  // ==========================================================
  const loadData = () => {
    fetchCategories()
      .then((res: any) => {
        // Tùy vào cách cấu hình Axios, dữ liệu có thể nằm trong res hoặc res.data
        const data = res.data ? res.data : res; 
        setCategories(data);
      })
      .catch((err) => {
        console.error("Lỗi khi tải danh sách danh mục:", err);
        alert("Không thể kết nối đến máy chủ Backend!");
      });
  };

  useEffect(() => {
    loadData(); // Gọi ngay khi vừa mở trang
  }, []);

  // ==========================================================
  // CÁC HÀM XỬ LÝ SỰ KIỆN FORM
  // ==========================================================

  // Tự động sinh Slug khi gõ Tên
  const handleNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newName = e.target.value;
    setName(newName);
    setSlug(newName.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").replace(/đ/g, "d").replace(/[^a-z0-9 ]/g, "").replace(/\s+/g, "-"));
  };

  // Nút: Mở Form Thêm Mới
  const handleOpenCreate = () => {
    setIsEditing(false);
    setEditCategoryId(null);
    setName("");
    setSlug("");
    setIsActive(true);
    setParentId("");
    setIsModalOpen(true);
  };

  // Nút: Mở Form Cập Nhật (Nạp dữ liệu cũ)
  const handleOpenEdit = (category: Category) => {
    setIsEditing(true);
    setEditCategoryId(category.id);
    setName(category.name);
    setSlug(category.slug);
    setIsActive(category.isActive);
    setParentId(category.parentId ? category.parentId.toString() : "");
    setIsModalOpen(true);
  };

  // ==========================================================
  // CÁC HÀM TƯƠNG TÁC API (Create, Update, Delete)
  // ==========================================================

  // Submit Form (Dùng chung cho cả Thêm và Sửa)
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const payloadData = { 
      name, 
      slug, 
      isActive, 
      parentId: parentId ? parseInt(parentId) : null 
    };

    if (isEditing && editCategoryId) {
      // ==== GỌI API CẬP NHẬT (PUT) ====
      updateCategory(editCategoryId, payloadData)
        .then(() => {
          alert("Cập nhật thành công!");
          loadData(); // Tải lại danh sách mới nhất
          setIsModalOpen(false); // Đóng form
        })
        .catch((err) => {
          console.error("Lỗi cập nhật:", err);
          alert("Cập nhật thất bại, vui lòng kiểm tra lại!");
        });

    } else {
      // ==== GỌI API THÊM MỚI (POST) ====
      createCategory(payloadData)
        .then(() => {
          alert("Thêm mới thành công!");
          loadData(); // Tải lại danh sách mới nhất
          setIsModalOpen(false); // Đóng form
        })
        .catch((err) => {
          console.error("Lỗi thêm mới:", err);
          alert("Thêm mới thất bại, vui lòng kiểm tra lại!");
        });
    }
  };

  // Xóa danh mục (DELETE)
  const handleDeleteCategory = (id: number) => {
    const isConfirm = window.confirm('Bạn có chắc chắn muốn xóa danh mục này?');
    if (!isConfirm) return;

    deleteCategory(id)
      .then(() => {
        // Có thể gọi loadData() để tải lại, hoặc filter state cho nhẹ frontend
        setCategories(categories.filter((cat) => cat.id !== id));
      })
      .catch((err) => {
        console.error("Lỗi khi xóa:", err);
        alert("Xóa thất bại! Có thể danh mục này đang chứa sản phẩm bên trong.");
      });
  };

  // ==========================================================
  // RENDER UI
  // ==========================================================
  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-100 relative">
      
      {/* HEADER BẢNG */}
      <div className="p-6 border-b border-gray-100 flex justify-between items-center">
        <h1 className="text-xl font-bold text-gray-800">Quản lý Danh mục</h1>
        <button 
          onClick={handleOpenCreate} 
          className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium text-sm transition-colors"
        >
          <Plus size={18} /> Thêm danh mục
        </button>
      </div>

      {/* DANH SÁCH BẢNG */}
      <div className="overflow-x-auto">
        <table className="w-full text-left border-collapse">
          <thead>
            <tr className="bg-gray-50 text-gray-500 text-sm border-b border-gray-100">
              <th className="py-3 px-6 w-2/5">Tên danh mục</th>
              <th className="py-3 px-6">Đường dẫn (Slug)</th>
              <th className="py-3 px-6">Trạng thái</th>
              <th className="py-3 px-6 w-24 text-center">Hành động</th>
            </tr>
          </thead>
          <tbody>
            {categories.map((cat) => (
              <tr key={cat.id} className="border-b border-gray-100 hover:bg-gray-50 transition-colors">
                <td className="py-3 px-6 font-medium text-gray-800">
                  {cat.parentId ? <span className="text-gray-400 mr-2">↳</span> : null}
                  {cat.name}
                </td>
                <td className="py-3 px-6 text-gray-500 text-sm">{cat.slug}</td>
                <td className="py-3 px-6">
                  <span className={clsx("px-3 py-1 text-xs font-medium rounded-full", cat.isActive ? "bg-green-100 text-green-700" : "bg-red-100 text-red-700")}>
                    {cat.isActive ? "Hiển thị" : "Đang ẩn"}
                  </span>
                </td>
                <td className="py-3 px-6 flex justify-center gap-2">
                  <button onClick={() => handleOpenEdit(cat)} className="text-blue-600 hover:bg-blue-50 p-1.5 rounded-md transition-colors" title="Sửa">
                    <Edit size={16} />
                  </button>
                  <button onClick={() => handleDeleteCategory(cat.id)} className="text-red-600 hover:bg-red-50 p-1.5 rounded-md transition-colors" title="Xóa">
                    <Trash2 size={16} />
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {categories.length === 0 && <div className="p-8 text-center text-gray-500">Đang tải dữ liệu hoặc chưa có danh mục nào...</div>}
      </div>

      {/* MODAL FORM */}
      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50 backdrop-blur-sm">
          <div className="bg-white rounded-xl shadow-2xl w-full max-w-md overflow-hidden animate-in fade-in zoom-in duration-200">
            
            <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50">
              <h3 className="font-bold text-lg text-gray-800">
                {isEditing ? "Cập nhật danh mục" : "Thêm danh mục mới"}
              </h3>
              <button onClick={() => setIsModalOpen(false)} className="text-gray-400 hover:text-red-500 transition-colors">
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Tên danh mục <span className="text-red-500">*</span></label>
                <input type="text" required value={name} onChange={handleNameChange} placeholder="VD: Giày Thể Thao" className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none transition-all" />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Đường dẫn (Slug) <span className="text-red-500">*</span></label>
                <input type="text" required value={slug} onChange={(e) => setSlug(e.target.value)} className="w-full px-4 py-2 border border-gray-300 rounded-lg bg-gray-50 focus:ring-2 focus:ring-blue-500 outline-none" />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Danh mục cha</label>
                <select value={parentId} onChange={(e) => setParentId(e.target.value)} className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none bg-white">
                  <option value="">-- Trở thành danh mục gốc --</option>
                  {/* Lọc bỏ chính danh mục đang sửa để tránh chọn tự làm cha của mình */}
                  {categories.filter(c => c.id !== editCategoryId).map(cat => (
                    <option key={cat.id} value={cat.id}>{cat.name}</option>
                  ))}
                </select>
              </div>

              <div className="flex items-center gap-2 pt-2">
                <input type="checkbox" id="isActive" checked={isActive} onChange={(e) => setIsActive(e.target.checked)} className="w-4 h-4 text-blue-600 rounded border-gray-300 focus:ring-blue-500" />
                <label htmlFor="isActive" className="text-sm font-medium text-gray-700 cursor-pointer">Hiển thị trên web</label>
              </div>

              <div className="pt-4 flex justify-end gap-3 border-t border-gray-100 mt-6">
                <button type="button" onClick={() => setIsModalOpen(false)} className="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg font-medium transition-colors">Hủy bỏ</button>
                <button type="submit" className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-medium transition-colors">
                  <Save size={18} /> {isEditing ? "Cập nhật" : "Lưu danh mục"}
                </button>
              </div>
            </form>

          </div>
        </div>
      )}

    </div>
  );
}