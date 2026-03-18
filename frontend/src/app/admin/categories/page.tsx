"use client";

import React, { useEffect, useState } from "react";
import { Edit, Plus, Save, Trash2, X } from "lucide-react";
import {
  type CategoryDto,
  createCategory,
  deleteCategory,
  fetchCategories,
  updateCategory,
} from "@/src/services/api";

export default function CategoryPage() {
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [editCategoryId, setEditCategoryId] = useState<number | null>(null);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");

  const loadData = () => {
    fetchCategories()
      .then((res) => {
        setCategories(res.data);
      })
      .catch((err) => {
        console.error("Lỗi khi tải danh sách danh mục:", err);
        alert("Không thể kết nối đến máy chủ Backend!");
      });
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleOpenCreate = () => {
    setIsEditing(false);
    setEditCategoryId(null);
    setName("");
    setDescription("");
    setIsModalOpen(true);
  };

  const handleOpenEdit = (category: CategoryDto) => {
    setIsEditing(true);
    setEditCategoryId(category.id);
    setName(category.name);
    setDescription(category.description ?? "");
    setIsModalOpen(true);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const payloadData = {
      name,
      description: description.trim() ? description.trim() : null,
    };

    if (isEditing && editCategoryId) {
      updateCategory(editCategoryId, payloadData)
        .then(() => {
          alert("Cập nhật thành công!");
          loadData();
          setIsModalOpen(false);
        })
        .catch((err) => {
          console.error("Lỗi cập nhật:", err);
          alert("Cập nhật thất bại, vui lòng kiểm tra lại!");
        });

      return;
    }

    createCategory(payloadData)
      .then(() => {
        alert("Thêm mới thành công!");
        loadData();
        setIsModalOpen(false);
      })
      .catch((err) => {
        console.error("Lỗi thêm mới:", err);
        alert("Thêm mới thất bại, vui lòng kiểm tra lại!");
      });
  };

  const handleDeleteCategory = (id: number) => {
    const isConfirm = window.confirm("Bạn có chắc chắn muốn xóa danh mục này?");
    if (!isConfirm) return;

    deleteCategory(id)
      .then(() => {
        setCategories(categories.filter((cat) => cat.id !== id));
      })
      .catch((err) => {
        console.error("Lỗi khi xóa:", err);
        alert("Xóa thất bại! Có thể danh mục này đang được sản phẩm sử dụng.");
      });
  };

  return (
    <div className="relative rounded-xl border border-gray-100 bg-white shadow-sm">
      <div className="flex items-center justify-between border-b border-gray-100 p-6">
        <h1 className="text-xl font-bold text-gray-800">Quản lý Danh mục</h1>
        <button
          onClick={handleOpenCreate}
          className="flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white transition-colors hover:bg-blue-700"
        >
          <Plus size={18} /> Thêm danh mục
        </button>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full border-collapse text-left">
          <thead>
            <tr className="border-b border-gray-100 bg-gray-50 text-sm text-gray-500">
              <th className="w-1/3 px-6 py-3">Tên danh mục</th>
              <th className="px-6 py-3">Mô tả</th>
              <th className="w-24 px-6 py-3 text-center">Hành động</th>
            </tr>
          </thead>
          <tbody>
            {categories.map((cat) => (
              <tr
                key={cat.id}
                className="border-b border-gray-100 transition-colors hover:bg-gray-50"
              >
                <td className="px-6 py-3 font-medium text-gray-800">{cat.name}</td>
                <td className="px-6 py-3 text-sm text-gray-500">
                  {cat.description || "Không có mô tả"}
                </td>
                <td className="flex justify-center gap-2 px-6 py-3">
                  <button
                    onClick={() => handleOpenEdit(cat)}
                    className="rounded-md p-1.5 text-blue-600 transition-colors hover:bg-blue-50"
                    title="Sửa"
                  >
                    <Edit size={16} />
                  </button>
                  <button
                    onClick={() => handleDeleteCategory(cat.id)}
                    className="rounded-md p-1.5 text-red-600 transition-colors hover:bg-red-50"
                    title="Xóa"
                  >
                    <Trash2 size={16} />
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {categories.length === 0 && (
          <div className="p-8 text-center text-gray-500">
            Đang tải dữ liệu hoặc chưa có danh mục nào...
          </div>
        )}
      </div>

      {isModalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50 backdrop-blur-sm">
          <div className="w-full max-w-md overflow-hidden rounded-xl bg-white shadow-2xl animate-in fade-in zoom-in duration-200">
            <div className="flex items-center justify-between border-b border-gray-100 bg-gray-50 px-6 py-4">
              <h3 className="text-lg font-bold text-gray-800">
                {isEditing ? "Cập nhật danh mục" : "Thêm danh mục mới"}
              </h3>
              <button
                onClick={() => setIsModalOpen(false)}
                className="text-gray-400 transition-colors hover:text-red-500"
              >
                <X size={20} />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-4 p-6">
              <div>
                <label className="mb-1 block text-sm font-medium text-gray-700">
                  Tên danh mục <span className="text-red-500">*</span>
                </label>
                <input
                  type="text"
                  required
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="VD: Giày Thể Thao"
                  className="w-full rounded-lg border border-gray-300 px-4 py-2 outline-none transition-all focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium text-gray-700">
                  Mô tả
                </label>
                <textarea
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                  rows={4}
                  className="w-full resize-none rounded-lg border border-gray-300 px-4 py-2 outline-none transition-all focus:ring-2 focus:ring-blue-500"
                />
              </div>

              <div className="mt-6 flex justify-end gap-3 border-t border-gray-100 pt-4">
                <button
                  type="button"
                  onClick={() => setIsModalOpen(false)}
                  className="rounded-lg px-4 py-2 font-medium text-gray-600 transition-colors hover:bg-gray-100"
                >
                  Hủy bỏ
                </button>
                <button
                  type="submit"
                  className="flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 font-medium text-white transition-colors hover:bg-blue-700"
                >
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
