"use client";

import { useState, useEffect, useRef } from "react";
import { CategoryService } from "@/src/services/categoryService";
import { Category } from "@/src/types/category";
import { PlusCircle, Trash2, Edit, X, FolderTree } from "lucide-react";

export default function CategoryManager() {
    const [categories, setCategories] = useState<Category[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const formRef = useRef<HTMLFormElement | null>(null);

    // Form State
    const [editingId, setEditingId] = useState<number | null>(null);
    const [tenDanhMuc, setTenDanhMuc] = useState("");
    const [moTa, setMoTa] = useState("");
    const [trangThai, setTrangThai] = useState<boolean>(true);

    const fetchData = async () => {
        try {
            setIsLoading(true);
            const data = await CategoryService.getAll();
            setCategories(data);
        } catch (error) {
            console.error(error);
            alert("Lỗi tải danh sách danh mục!");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!tenDanhMuc.trim()) return alert("Tên danh mục không được để trống!");

        const requestData = { 
            tenDanhMuc: tenDanhMuc.trim(), 
            moTa: moTa.trim(), 
            trangThai 
        };

        try {
            if (editingId === null) {
                await CategoryService.create(requestData);
                alert("Thêm danh mục thành công!");
            } else {
                if (!confirm("Bạn có chắc chắn muốn cập nhật danh mục này?")) return;
                await CategoryService.update(editingId, requestData);
                alert("Cập nhật danh mục thành công!");
            }
            resetForm();
            fetchData();
        } catch (error: any) {
            alert("Lỗi: " + (error?.response?.data?.title || error?.response?.data || "Đã có lỗi xảy ra"));
        }
    };

    const handleEditClick = (cat: Category) => {
        setEditingId(cat.id);
        setTenDanhMuc(cat.tenDanhMuc);
        setMoTa(cat.moTa || "");
        setTrangThai(cat.trangThai);
        formRef.current?.scrollIntoView({ behavior: "smooth", block: "start" });
    };

    const handleDelete = async (id: number, soSanPham: number) => {
        if (soSanPham > 0) {
            return alert(`Không thể xóa! Danh mục này đang chứa ${soSanPham} sản phẩm.`);
        }
        
        if (!confirm("Bạn có chắc chắn muốn xóa danh mục này?")) return;
        
        try {
            await CategoryService.delete(id);
            fetchData();
        } catch (error: any) {
            alert("Lỗi xóa: " + (error?.response?.data?.title || "Không thể xóa"));
        }
    };

    const resetForm = () => {
        setEditingId(null);
        setTenDanhMuc("");
        setMoTa("");
        setTrangThai(true);
    };

    return (
        <div className="max-w-5xl mx-auto p-6 space-y-8">
            <h1 className="text-3xl font-bold text-gray-800 flex items-center gap-3">
                <FolderTree className="text-blue-600" size={32} /> Quản lý Danh Mục
            </h1>

            {/* FORM LƯỠNG TÍNH */}
            <form ref={formRef} onSubmit={handleSubmit} className={`bg-white p-6 rounded-xl shadow-sm border-t-4 transition-colors ${editingId ? 'border-orange-500' : 'border-blue-500'}`}>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div className="space-y-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Tên Danh Mục *</label>
                            <input type="text" value={tenDanhMuc} onChange={e => setTenDanhMuc(e.target.value)} placeholder="VD: Giày Thể Thao" className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 outline-none" />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">Trạng Thái</label>
                            <div className="flex items-center gap-6 mt-2">
                                <label className="flex items-center gap-2 cursor-pointer">
                                    <input 
                                        type="radio" 
                                        name="trangThai" 
                                        checked={trangThai === true} 
                                        onChange={() => setTrangThai(true)} 
                                        className="w-4 h-4 text-blue-600 cursor-pointer"
                                    />
                                    <span className="text-sm font-medium text-gray-800"> Đang hoạt động</span>
                                </label>
                                
                                <label className="flex items-center gap-2 cursor-pointer">
                                    <input 
                                        type="radio" 
                                        name="trangThai" 
                                        checked={trangThai === false} 
                                        onChange={() => setTrangThai(false)} 
                                        className="w-4 h-4 text-red-600 cursor-pointer"
                                    />
                                    <span className="text-sm font-medium text-gray-800"> Đã ẩn</span>
                                </label>
                            </div>
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Mô tả (Tùy chọn)</label>
                        <textarea value={moTa} onChange={e => setMoTa(e.target.value)} rows={4} placeholder="Nhập mô tả cho danh mục này..." className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-blue-500 outline-none resize-none" />
                    </div>
                </div>

                <div className="mt-6 flex justify-end gap-3 pt-4">
                    {editingId && (
                        <button type="button" onClick={resetForm} className="px-6 py-2 rounded-lg font-bold bg-gray-200 text-gray-700 hover:bg-gray-300 flex items-center gap-2">
                            <X size={18} /> Hủy
                        </button>
                    )}
                    <button type="submit" className={`px-8 py-2 rounded-lg font-bold text-white flex items-center gap-2 ${editingId ? 'bg-orange-600 hover:bg-orange-700' : 'bg-blue-600 hover:bg-blue-700'}`}>
                        {editingId ? 'Lưu Thay Đổi' : 'Thêm Danh Mục'}
                    </button>
                </div>
            </form>

            {/* BẢNG DỮ LIỆU */}
            <div className="bg-white rounded-xl shadow-sm border overflow-hidden">
                <table className="w-full text-left border-collapse">
                    <thead className="bg-gray-50 text-gray-700 border-b">
                        <tr>
                            <th className="py-3 px-4 w-16 text-center">STT</th>
                            <th className="py-3 px-4">Tên Danh Mục</th>
                            <th className="py-3 px-4">Mô Tả</th>
                            <th className="py-3 px-4 text-center">Số SP</th>
                            <th className="py-3 px-4 text-center">Trạng Thái</th>
                            <th className="py-3 px-4 text-center w-32">Hành Động</th>
                        </tr>
                    </thead>
                    <tbody>
                        {isLoading ? (
                            <tr><td colSpan={6} className="text-center py-8 text-gray-500">Đang tải dữ liệu...</td></tr>
                        ) : categories.length === 0 ? (
                            <tr><td colSpan={6} className="text-center py-8 text-gray-500">Chưa có danh mục nào.</td></tr>
                        ) : (
                            categories.map((cat, index) => (
                                <tr
                                    key={cat.id}
                                    onClick={() => handleEditClick(cat)}
                                    className={`border-b last:border-0 cursor-pointer transition-colors ${editingId === cat.id ? "bg-orange-50 border-orange-200" : "hover:bg-gray-50"}`}
                                >
                                    <td className="py-3 px-4 text-center text-gray-500">{index + 1}</td>
                                    <td className="py-3 px-4 font-bold text-gray-800">{cat.tenDanhMuc}</td>
                                    <td className="py-3 px-4 text-gray-600 text-sm truncate max-w-xs" title={cat.moTa || ""}>
                                        {cat.moTa || "-"}
                                    </td>
                                    <td className="py-3 px-4 text-center">
                                        <span className={`px-2 py-1 rounded-full text-xs font-bold ${cat.soSanPham > 0 ? 'bg-blue-100 text-blue-700' : 'bg-gray-100 text-gray-500'}`}>
                                            {cat.soSanPham}
                                        </span>
                                    </td>
                                    <td className="py-3 px-4 text-center">
                                        {cat.trangThai === true ? (
                                            <span className="text-green-600 text-sm font-semibold">Hoạt động</span>
                                        ) : (
                                            <span className="text-red-500 text-sm font-semibold">Đã ẩn</span>
                                        )}
                                    </td>
                                    <td className="py-3 px-4 flex justify-center gap-2">
                                        <button
                                            onClick={(e) => {
                                                e.stopPropagation();
                                                handleDelete(cat.id, cat.soSanPham);
                                            }}
                                            className="p-2 text-red-500 hover:bg-red-50 rounded-lg"
                                        >
                                            <Trash2 size={18} />
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
