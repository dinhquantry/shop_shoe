"use client";

import { useState, useEffect, useRef } from "react";
import { BrandService } from "@/src/services/brandService";
import { Brand } from "@/src/types/brand";
import { PlusCircle, Trash2, Edit, X, Award } from "lucide-react";

export default function BrandManager() {
    const [brands, setBrands] = useState<Brand[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const formRef = useRef<HTMLFormElement | null>(null);

    // Form State
    const [editingId, setEditingId] = useState<number | null>(null);
    const [tenThuongHieu, setTenThuongHieu] = useState("");
    const [logoUrl, setLogoUrl] = useState("");
    const [moTa, setMoTa] = useState("");
    const [trangThai, setTrangThai] = useState<boolean>(true);

    const fetchData = async () => {
        try {
            setIsLoading(true);
            const data = await BrandService.getAll();
            setBrands(data);
        } catch (error) {
            console.error(error);
            alert("Lỗi tải danh sách thương hiệu!");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!tenThuongHieu.trim()) return alert("Tên thương hiệu không được để trống!");

        const requestData = { 
            tenThuongHieu: tenThuongHieu.trim(), 
            logoUrl: logoUrl.trim(),
            moTa: moTa.trim(), 
            trangThai 
        };

        try {
            if (editingId === null) {
                await BrandService.create(requestData);
                alert("Thêm thương hiệu thành công!");
            } else {
                if (!confirm("Báº¡n cÃ³ cháº¯c cháº¯n muá»‘n cáº­p nháº­t thÆ°Æ¡ng hiá»‡u nÃ y?")) return;
                await BrandService.update(editingId, requestData);
                alert("Cập nhật thương hiệu thành công!");
            }
            resetForm();
            fetchData();
        } catch (error: any) {
            alert("Lỗi: " + (error?.response?.data?.title || "Đã có lỗi xảy ra"));
        }
    };

    const handleEditClick = (brand: Brand) => {
        setEditingId(brand.id);
        setTenThuongHieu(brand.tenThuongHieu);
        setLogoUrl(brand.logoUrl || "");
        setMoTa(brand.moTa || "");
        setTrangThai(brand.trangThai);
        formRef.current?.scrollIntoView({ behavior: "smooth", block: "start" });
    };

    const handleDelete = async (id: number, soSanPham: number) => {
        if (soSanPham > 0) {
            return alert(`Không thể xóa! Thương hiệu này đang có ${soSanPham} sản phẩm.`);
        }
        
        if (!confirm("Bạn có chắc chắn muốn xóa thương hiệu này?")) return;
        
        try {
            await BrandService.delete(id);
            fetchData();
        } catch (error: any) {
            alert("Lỗi xóa: " + (error?.response?.data?.title || "Không thể xóa"));
        }
    };

    const resetForm = () => {
        setEditingId(null);
        setTenThuongHieu("");
        setLogoUrl("");
        setMoTa("");
        setTrangThai(true);
    };

    return (
        <div className="max-w-6xl mx-auto p-6 space-y-8">
            <h1 className="text-3xl font-bold text-gray-800 flex items-center gap-3">
                <Award className="text-purple-600" size={32} /> Quản lý Thương Hiệu
            </h1>

            {/* FORM */}
            <form ref={formRef} onSubmit={handleSubmit} className={`bg-white p-6 rounded-xl shadow-sm transition-colors border-2 ${editingId ? 'border-orange-400' : 'border-purple-400'}`}>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div className="space-y-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Tên Thương Hiệu *</label>
                            <input type="text" value={tenThuongHieu} onChange={e => setTenThuongHieu(e.target.value)} placeholder="VD: Nike, Adidas..." className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-purple-500 outline-none" />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Link Ảnh Logo (Tùy chọn)</label>
                            <input type="text" value={logoUrl} onChange={e => setLogoUrl(e.target.value)} placeholder="https://..." className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-purple-500 outline-none" />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-2">Trạng Thái</label>
                            <div className="flex items-center gap-6 mt-2">
                                <label className="flex items-center gap-2 cursor-pointer">
                                    <input type="radio" checked={trangThai === true} onChange={() => setTrangThai(true)} className="w-4 h-4 text-purple-600" />
                                    <span className="text-sm font-medium text-gray-800">Đang hoạt động</span>
                                </label>
                                <label className="flex items-center gap-2 cursor-pointer">
                                    <input type="radio" checked={trangThai === false} onChange={() => setTrangThai(false)} className="w-4 h-4 text-red-600" />
                                    <span className="text-sm font-medium text-gray-800">Đã ẩn</span>
                                </label>
                            </div>
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Mô tả (Tùy chọn)</label>
                        <textarea value={moTa} onChange={e => setMoTa(e.target.value)} rows={6} placeholder="Nhập mô tả về thương hiệu..." className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-purple-500 outline-none resize-none" />
                    </div>
                </div>

                <div className="mt-6 flex justify-end gap-3 pt-4">
                    {editingId && (
                        <button type="button" onClick={resetForm} className="px-6 py-2 rounded-lg font-bold bg-gray-200 text-gray-700 hover:bg-gray-300 flex items-center gap-2">
                            <X size={18} /> Hủy
                        </button>
                    )}
                    <button type="submit" className={`px-8 py-2 rounded-lg font-bold text-white flex items-center gap-2 ${editingId ? 'bg-orange-600 hover:bg-orange-700' : 'bg-purple-600 hover:bg-purple-700'}`}>
                        {editingId ? 'Lưu Thay Đổi' : 'Thêm Thương Hiệu'}
                    </button>
                </div>
            </form>

            {/* BẢNG DỮ LIỆU */}
            <div className="bg-white rounded-xl shadow-sm border overflow-hidden">
                <table className="w-full text-left border-collapse">
                    <thead className="bg-gray-50 text-gray-700 border-b">
                        <tr>
                            <th className="py-3 px-4 w-16 text-center">STT</th>
                            <th className="py-3 px-4 text-center w-24">Logo</th>
                            <th className="py-3 px-4">Tên Thương Hiệu</th>
                            <th className="py-3 px-4">Mô Tả</th>
                            <th className="py-3 px-4 text-center">Số SP</th>
                            <th className="py-3 px-4 text-center">Trạng Thái</th>
                            <th className="py-3 px-4 text-center w-32">Hành Động</th>
                        </tr>
                    </thead>
                    <tbody>
                        {isLoading ? (
                            <tr><td colSpan={7} className="text-center py-8 text-gray-500">Đang tải dữ liệu...</td></tr>
                        ) : brands.length === 0 ? (
                            <tr><td colSpan={7} className="text-center py-8 text-gray-500">Chưa có thương hiệu nào.</td></tr>
                        ) : (
                            brands.map((brand, index) => (
                                <tr key={brand.id} onClick={() => handleEditClick(brand)} className={`border-b last:border-0 cursor-pointer transition-colors ${editingId === brand.id ? 'bg-orange-50 border-orange-200' : 'hover:bg-gray-50'}`}>
                                    <td className="py-3 px-4 text-center text-gray-500">{index + 1}</td>
                                    
                                    {/* Hiển thị Logo */}
                                    <td className="py-3 px-4 text-center">
                                        {brand.logoUrl ? (
                                            <img src={brand.logoUrl} alt={brand.tenThuongHieu} className="h-10 w-16 object-contain mx-auto bg-gray-50 rounded" />
                                        ) : (
                                            <span className="text-xs text-gray-400">Không có</span>
                                        )}
                                    </td>

                                    <td className="py-3 px-4 font-bold text-gray-800">{brand.tenThuongHieu}</td>
                                    <td className="py-3 px-4 text-gray-600 text-sm truncate max-w-xs" title={brand.moTa || ""}>{brand.moTa || "-"}</td>
                                    <td className="py-3 px-4 text-center">
                                        <span className={`px-2 py-1 rounded-full text-xs font-bold ${brand.soSanPham > 0 ? 'bg-purple-100 text-purple-700' : 'bg-gray-100 text-gray-500'}`}>
                                            {brand.soSanPham}
                                        </span>
                                    </td>
                                    <td className="py-3 px-4 text-center">
                                        {brand.trangThai ? (
                                            <span className="text-green-600 text-sm font-semibold">Hoạt động</span>
                                        ) : (
                                            <span className="text-red-500 text-sm font-semibold">Đã ẩn</span>
                                        )}
                                    </td>
                                    <td className="py-3 px-4 flex justify-center gap-2">
                                        <button onClick={(e) => { e.stopPropagation(); handleDelete(brand.id, brand.soSanPham); }} className="p-2 text-red-500 hover:bg-red-50 rounded-lg">
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
