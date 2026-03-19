"use client";

import { useState, useEffect, useRef } from "react";

import { Trash2, PlusCircle, AlertCircle, Edit, X, Image as ImageIcon, Upload } from "lucide-react";
import { Brand } from "@/src/types/brand";
import { BrandService } from "@/src/services/brandService";

// LƯU Ý: Đổi cái link này thành đúng số Port C# của em (nếu nó khác 5266)
const BACKEND_URL = "http://localhost:5266"; 

export default function BrandManager() {
    const [brands, setBrands] = useState<Brand[]>([]);
    const [isLoading, setIsLoading] = useState(true);

    // State Form Lưỡng tính
    const [editingId, setEditingId] = useState<number | null>(null);
    const [tenTh, setTenTh] = useState("");
    const [moTa, setMoTa] = useState("");
    
    // State Xử lý File Logo
    const [logoFile, setLogoFile] = useState<File | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    const fetchData = async () => {
        try {
            setIsLoading(true);
            const data = await BrandService.getAll();
            setBrands(data);
        } catch (error) {
            console.error(error);
            alert("Lỗi kết nối Backend!");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, []);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!tenTh.trim()) {
            alert("Tên thương hiệu không được để trống!");
            return;
        }

        try {
            let brandId = editingId;

            if (editingId === null) {
                // THÊM MỚI (Bước 1: Tạo chữ)
                const newBrand = await BrandService.create({ tenTh, moTa });
                brandId = newBrand.maTh; // Lấy ID vừa tạo
            } else {
                // CẬP NHẬT (Bước 1: Sửa chữ)
                await BrandService.update(editingId, { tenTh, moTa });
            }

            // Bước 2: Nếu người dùng có chọn File Logo mới thì up lên
            if (logoFile && brandId) {
                await BrandService.uploadLogo(brandId, logoFile);
            }

            alert(editingId ? "Cập nhật thành công!" : "Thêm thương hiệu thành công!");
            resetForm();
            fetchData();
        } catch (error: any) {
            alert("Lỗi: " + (error?.response?.data || error.message));
        }
    };

    const handleEditClick = (brand: Brand) => {
        setEditingId(brand.maTh);
        setTenTh(brand.tenTh);
        setMoTa(brand.moTa || "");
        setLogoFile(null); // Reset file rác
        if (fileInputRef.current) fileInputRef.current.value = "";
        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    const handleDelete = async (id: number) => {
        if (!confirm("Xác nhận xóa thương hiệu này? (Sẽ bị chặn nếu đang có giày)")) return;
        try {
            await BrandService.delete(id);
            fetchData();
        } catch (error: any) {
            alert("Lỗi khi xóa: " + (error?.response?.data || error.message));
        }
    };

    const resetForm = () => {
        setEditingId(null);
        setTenTh("");
        setMoTa("");
        setLogoFile(null);
        if (fileInputRef.current) fileInputRef.current.value = "";
    };

    return (
        <div className="max-w-5xl mx-auto p-6">
            <h1 className="text-3xl font-bold text-gray-800 mb-6">Quản lý Thương Hiệu</h1>

            {/* FORM LƯỠNG TÍNH */}
            <form onSubmit={handleSubmit} className="bg-white p-6 rounded-xl shadow-md border border-gray-100 mb-8">
                <h2 className={`text-lg font-semibold mb-6 flex items-center gap-2 ${editingId ? 'text-orange-600' : 'text-gray-700'}`}>
                    {editingId ? <Edit size={20} /> : <PlusCircle className="text-blue-500" size={20} />}
                    {editingId ? `Đang sửa Thương Hiệu #${editingId}` : 'Thêm Thương Hiệu Mới'}
                </h2>
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div className="space-y-4">
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Tên Thương Hiệu *</label>
                            <input type="text" value={tenTh} onChange={(e) => setTenTh(e.target.value)} placeholder="VD: Nike, Adidas..." className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none" />
                        </div>
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1">Mô tả ngắn</label>
                            <input type="text" value={moTa} onChange={(e) => setMoTa(e.target.value)} placeholder="Mô tả..." className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none" />
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">Upload Logo (Tuỳ chọn)</label>
                        <div className="border-2 border-dashed border-gray-300 rounded-lg p-4 text-center hover:bg-gray-50 h-full flex flex-col justify-center">
                            <input 
                                type="file" 
                                accept="image/*"
                                ref={fileInputRef}
                                onChange={e => setLogoFile(e.target.files ? e.target.files[0] : null)}
                                className="block w-full text-sm text-gray-500 file:mr-4 file:py-2 file:px-4 file:rounded-md file:border-0 file:text-sm file:font-semibold file:bg-blue-50 file:text-blue-700 hover:file:bg-blue-100 cursor-pointer"
                            />
                            {logoFile && <p className="mt-2 text-sm text-green-600 font-medium whitespace-nowrap overflow-hidden text-ellipsis">Đã chọn: {logoFile.name}</p>}
                            {!logoFile && editingId && <p className="mt-2 text-sm text-orange-500">Bỏ trống nếu giữ nguyên Logo cũ</p>}
                        </div>
                    </div>
                </div>

                <div className="mt-6 flex justify-end gap-3 border-t pt-4">
                    {editingId && (
                        <button type="button" onClick={resetForm} className="flex items-center gap-1 bg-gray-200 hover:bg-gray-300 text-gray-700 px-6 py-2 rounded-lg transition-colors font-medium">
                            <X size={18} /> Hủy Sửa
                        </button>
                    )}
                    <button type="submit" className={`flex items-center gap-2 px-8 py-2 rounded-lg font-bold transition-colors text-white ${editingId ? 'bg-orange-500 hover:bg-orange-600' : 'bg-blue-600 hover:bg-blue-700'}`}>
                        {editingId ? <Edit size={18} /> : <Upload size={18} />}
                        {editingId ? 'Lưu Thay Đổi' : 'Thêm & Up Logo'}
                    </button>
                </div>
            </form>

            {/* BẢNG DỮ LIỆU */}
            <div className="bg-white rounded-xl shadow-md border border-gray-100 overflow-hidden">
                <table className="w-full text-left border-collapse">
                    <thead className="bg-gray-50 text-gray-700 border-b border-gray-200">
                        <tr>
                            <th className="py-3 px-4 w-16 text-center">ID</th>
                            <th className="py-3 px-4 w-24 text-center">Logo</th>
                            <th className="py-3 px-4">Tên Thương Hiệu</th>
                            <th className="py-3 px-4">Mô Tả</th>
                            <th className="py-3 px-4 w-32 text-center">Hành Động</th>
                        </tr>
                    </thead>
                    <tbody>
                        {isLoading ? (
                            <tr><td colSpan={5} className="text-center py-8 text-gray-500">Đang tải dữ liệu...</td></tr>
                        ) : brands.length === 0 ? (
                            <tr><td colSpan={5} className="text-center py-8 text-gray-500">Chưa có thương hiệu nào.</td></tr>
                        ) : (
                            brands.map((brand) => (
                                <tr key={brand.maTh} className="hover:bg-gray-50 border-b last:border-0 border-gray-100">
                                    <td className="py-3 px-4 text-center font-medium text-gray-500">#{brand.maTh}</td>
                                    
                                    {/* HIỂN THỊ LOGO */}
                                    <td className="py-3 px-4 text-center">
                                        {brand.logo ? (
                                            <img 
                                                src={`${BACKEND_URL}${brand.logo}`} 
                                                alt={brand.tenTh} 
                                                className="w-12 h-12 object-contain bg-white rounded border p-1 shadow-sm mx-auto"
                                                onError={(e) => {
                                                    // Nếu link ảnh hỏng, tự đổi sang icon mặc định
                                                    (e.target as HTMLImageElement).src = 'https://via.placeholder.com/48?text=No+Img';
                                                }}
                                            />
                                        ) : (
                                            <div className="w-12 h-12 bg-gray-100 rounded border flex items-center justify-center mx-auto text-gray-400">
                                                <ImageIcon size={20} />
                                            </div>
                                        )}
                                    </td>

                                    <td className="py-3 px-4 font-bold text-gray-800">{brand.tenTh}</td>
                                    <td className="py-3 px-4 text-gray-600">{brand.moTa || "-"}</td>
                                    <td className="py-3 px-4 flex justify-center gap-2 items-center h-full pt-6">
                                        <button onClick={() => handleEditClick(brand)} className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg" title="Sửa">
                                            <Edit size={20} />
                                        </button>
                                        <button onClick={() => handleDelete(brand.maTh)} className="p-2 text-red-500 hover:bg-red-50 rounded-lg" title="Xóa">
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