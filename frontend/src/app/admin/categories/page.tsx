"use client";
import { useState, useEffect } from "react";

import { Trash2, PlusCircle, AlertCircle, Edit, X } from "lucide-react"; // Thêm icon Edit và X
import { Category } from "@/src/types/category";
import { CategoryService } from "@/src/services/categoryService";

export default function CategoryManager() {
    const [categories, setCategories] = useState<Category[]>([]);
    const [tenDm, setTenDm] = useState("");
    const [moTa, setMoTa] = useState("");
    const [isLoading, setIsLoading] = useState(true);

   // hàm sửa
    const [editingId, setEditingId] = useState<number | null>(null);

    const fetchData = async () => {
        try {
            setIsLoading(true);
            const data = await CategoryService.getAll();
            setCategories(data);
        } catch (error) {
            console.error(error);
            alert("Không thể kết nối đến Backend C#!");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    //hàm chung sửa và thêm
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!tenDm.trim()) {
            alert("Tên danh mục không được để trống!");
            return;
        }

        try {
            if (editingId === null) {
                await CategoryService.create({ tenDm, moTa });
                alert("Thêm danh mục thành công!");
            } else {
                await CategoryService.update(editingId, { tenDm, moTa });
                alert("Cập nhật danh mục thành công!");
            }
            
            resetForm(); 
            fetchData(); 
        } catch (error: any) {
            alert("Lỗi: " + (error?.response?.data || error.message));
        }
    };

    // Hàm khi bấm nút "Sửa" ở dưới bảng
    const handleEditClick = (cat: Category) => {
        setEditingId(cat.maDm);      
        setTenDm(cat.tenDm);         
        setMoTa(cat.moTa || "");     
        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    const resetForm = () => {
        setEditingId(null);
        setTenDm("");
        setMoTa("");
    };

    //xóa
    const handleDelete = async (id: number) => {
        if (!confirm("Xác nhận xóa danh mục này?")) return;
        try {
            await CategoryService.delete(id);
            fetchData();
        } catch (error: any) {
            alert("Lỗi khi xóa: " + (error?.response?.data || error.message));
        }
    };

    return (
        <div className="max-w-5xl mx-auto p-6">
            <h1 className="text-3xl font-bold text-gray-800 mb-6">Quản lý Danh Mục</h1>

            <form onSubmit={handleSubmit} className="bg-white p-6 rounded-xl shadow-md border border-gray-100 mb-8 transition-all">
                <h2 className={`text-lg font-semibold mb-4 flex items-center gap-2 ${editingId ? 'text-orange-600' : 'text-gray-700'}`}>
                    {editingId ? <Edit size={20} /> : <PlusCircle className="text-blue-500" size={20} />}
                    {editingId ? `Đang sửa Danh Mục #${editingId}` : 'Thêm Danh Mục Mới'}
                </h2>
                
                <div className="flex gap-4 items-center">
                    <input
                        type="text"
                        placeholder="Tên danh mục..."
                        value={tenDm}
                        onChange={(e) => setTenDm(e.target.value)}
                        className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none"
                    />
                    <input
                        type="text"
                        placeholder="Mô tả..."
                        value={moTa}
                        onChange={(e) => setMoTa(e.target.value)}
                        className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none"
                    />
                    
                    <button 
                        type="submit"
                        className={`px-6 py-2 rounded-lg font-medium transition-colors text-white ${editingId ? 'bg-orange-500 hover:bg-orange-600' : 'bg-blue-600 hover:bg-blue-700'}`}
                    >
                        {editingId ? 'Lưu Cập Nhật' : 'Thêm Mới'}
                    </button>

                    {editingId && (
                        <button 
                            type="button" 
                            onClick={resetForm}
                            className="flex items-center gap-1 bg-gray-200 hover:bg-gray-300 text-gray-700 px-4 py-2 rounded-lg transition-colors"
                        >
                            <X size={18} /> Hủy
                        </button>
                    )}
                </div>
            </form>

            {/* BẢNG DỮ LIỆU */}
            <div className="bg-white rounded-xl shadow-md border border-gray-100 overflow-hidden">
                <table className="w-full text-left border-collapse">
                    <thead className="bg-gray-50 text-gray-700 border-b border-gray-200">
                        <tr>
                            <th className="py-3 px-4 w-24 text-center">Mã ID</th>
                            <th className="py-3 px-4">Tên Danh Mục</th>
                            <th className="py-3 px-4">Mô Tả</th>
                            <th className="py-3 px-4 w-40 text-center">Hành Động</th>
                        </tr>
                    </thead>
                    <tbody>
                        {isLoading ? (
                            <tr><td colSpan={4} className="text-center py-8 text-gray-500">Đang tải dữ liệu...</td></tr>
                        ) : categories.length === 0 ? (
                            <tr><td colSpan={4} className="text-center py-8 text-gray-500">Chưa có danh mục nào.</td></tr>
                        ) : (
                            categories.map((cat) => (
                                <tr key={cat.maDm} className="hover:bg-gray-50 transition-colors border-b last:border-0 border-gray-100">
                                    <td className="py-3 px-4 text-center font-medium text-gray-500">#{cat.maDm}</td>
                                    <td className="py-3 px-4 font-semibold text-gray-800">{cat.tenDm}</td>
                                    <td className="py-3 px-4 text-gray-600">{cat.moTa || "-"}</td>
                                    <td className="py-3 px-4 flex justify-center gap-2">
                                        <button
                                            onClick={() => handleEditClick(cat)}
                                            className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                                            title="Sửa danh mục"
                                        >
                                            <Edit size={20} />
                                        </button>

                                        <button
                                            onClick={() => handleDelete(cat.maDm)}
                                            className="p-2 text-red-500 hover:bg-red-50 rounded-lg transition-colors"
                                            title="Xóa danh mục"
                                        >
                                            <Trash2 size={20} />
                                        </button>
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}