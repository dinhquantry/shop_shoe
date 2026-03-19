"use client";

import { useState, useEffect, useRef } from "react";
import { ProductService } from "@/src/services/productService";
import { CategoryService } from "@/src/services/categoryService";
import { BrandService } from "@/src/services/brandService";
import { VariantService } from "@/src/services/variantService";
import { SizeService } from "@/src/services/sizeService";
import { ColorService } from "@/src/services/colorService";
import axiosClient from "@/src/services/axiosClient";
import { Product } from "@/src/types/product";
import { Variant } from "@/src/types/variant";
import { PlusCircle, Trash2, Edit, Upload, Package, X } from "lucide-react";

export default function ProductManager() {
    // ==========================================
    // 1. STATE TOÀN CỤC
    // ==========================================
    const [isLoading, setIsLoading] = useState(true);
    const [products, setProducts] = useState<Product[]>([]);
    const [categories, setCategories] = useState<any[]>([]);
    const [brands, setBrands] = useState<any[]>([]); // THÊM STATE THƯƠNG HIỆU
    const [sizes, setSizes] = useState<any[]>([]);
    const [colors, setColors] = useState<any[]>([]);

    // STATE: SẢN PHẨM (CHA)
    const [editingProductId, setEditingProductId] = useState<number | null>(null);
    const [tenSp, setTenSp] = useState("");
    const [donGia, setDonGia] = useState("");
    const [giaKhuyenMai, setGiaKhuyenMai] = useState("");
    const [moTa, setMoTa] = useState("");
    const [maDm, setMaDm] = useState<number>(0);
    const [maTh, setMaTh] = useState<number>(0); // KHÔNG CÒN FIX CỨNG NỮA
    const [imageFile, setImageFile] = useState<File | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    // STATE: BIẾN THỂ (CON)
    const [variants, setVariants] = useState<Variant[]>([]);
    const [maSize, setMaSize] = useState<number>(0);
    const [maMau, setMaMau] = useState<number>(0);
    const [soLuongTon, setSoLuongTon] = useState("");
    const [sku, setSku] = useState("");

    // ==========================================
    // 2. TẢI DỮ LIỆU BAN ĐẦU
    // ==========================================
    const fetchInitialData = async () => {
        try {
            setIsLoading(true);
            const [prods, cats, brds, szs, cols] = await Promise.all([
                ProductService.getAll(),
                CategoryService.getAll(),
                BrandService.getAll(), // KÉO DỮ LIỆU THƯƠNG HIỆU
                SizeService.getAll(),
                ColorService.getAll()
            ]);
            setProducts(prods);
            setCategories(cats);
            setBrands(brds); // LƯU VÀO STATE
            setSizes(szs);
            setColors(cols);

            setMaDm(current => {
                if (current !== 0 && cats.some(cat => cat.maDm === current)) return current;
                return cats[0]?.maDm ?? 0;
            });
            setMaTh(current => {
                if (current !== 0 && brds.some(brand => brand.maTh === current)) return current;
                return brds[0]?.maTh ?? 0;
            });
            setMaSize(current => {
                if (current !== 0 && szs.some(size => size.maSize === current)) return current;
                return szs[0]?.maSize ?? 0;
            });
            setMaMau(current => {
                if (current !== 0 && cols.some(color => color.maMau === current)) return current;
                return cols[0]?.maMau ?? 0;
            });
        } catch (error) {
            console.error(error);
            alert("Lỗi tải dữ liệu. C# đã chạy chưa?");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        fetchInitialData();
    }, []);

    const fetchVariantsForProduct = async (productId: number) => {
        try {
            const data = await axiosClient.get(`/variants/product/${productId}`);
            setVariants(data as any);
        } catch (error) {
            console.error(error);
        }
    };

    // ==========================================
    // 3. XỬ LÝ NGHIỆP VỤ SẢN PHẨM (CHA)
    // ==========================================
    const handleProductSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!tenSp.trim() || !donGia || maDm === 0 || maTh === 0) return alert("Nhập đủ Tên, Giá, Danh mục và Thương hiệu!");

        const parsedDonGia = Number(donGia);
        const parsedGiaKhuyenMai = giaKhuyenMai ? Number(giaKhuyenMai) : null;

        if (Number.isNaN(parsedDonGia) || parsedDonGia <= 0) return alert("Giá bán phải lớn hơn 0.");
        if (parsedGiaKhuyenMai !== null && (Number.isNaN(parsedGiaKhuyenMai) || parsedGiaKhuyenMai < 0)) return alert("Giá khuyến mãi không được nhỏ hơn 0.");
        if (parsedGiaKhuyenMai !== null && parsedGiaKhuyenMai > parsedDonGia) return alert("Giá khuyến mãi không được lớn hơn giá bán.");

        try {
            const productData = {
                tenSp: tenSp.trim(),
                donGia: parsedDonGia,
                giaKhuyenMai: parsedGiaKhuyenMai,
                moTa: moTa.trim() || null,
                maDm,
                maTh
            };

            let savedProductId = editingProductId;
            const isCreating = editingProductId === null;

            if (isCreating) {
                const newProduct = await ProductService.create(productData);
                savedProductId = newProduct.maSp;
                setEditingProductId(newProduct.maSp);
            } else {
                await ProductService.update(editingProductId, productData);
            }

            let message = isCreating ? "Đăng sản phẩm thành công!" : "Cập nhật thông tin thành công!";

            if (imageFile && savedProductId) {
                try {
                    await ProductService.uploadImage(savedProductId, imageFile, true);
                    setImageFile(null);
                    if (fileInputRef.current) fileInputRef.current.value = "";
                    message += " Ảnh đã được tải lên.";
                } catch (imageError: any) {
                    await fetchInitialData();
                    await fetchVariantsForProduct(savedProductId);
                    alert(`${message} Nhưng upload ảnh thất bại: ${imageError?.response?.data || imageError.message}`);
                    return;
                }
            }

            await fetchInitialData();
            if (savedProductId) {
                await fetchVariantsForProduct(savedProductId);
                setEditingProductId(savedProductId);
            }

            alert(isCreating ? `${message} Bạn có thể thêm phân loại kho ngay bên dưới.` : message);
        } catch (error: any) {
            alert("Lỗi: " + (error?.response?.data || error.message));
        }
    };

    const handleEditProduct = (p: Product) => {
        setEditingProductId(p.maSp);
        setTenSp(p.tenSp);
        setDonGia(p.donGia.toString());
        setGiaKhuyenMai(p.giaKhuyenMai?.toString() || "");
        setMoTa(p.moTa || "");
        setMaDm(p.maDm ?? 0);
        setMaTh(p.maTh ?? 0);
        setImageFile(null);
        if (fileInputRef.current) fileInputRef.current.value = "";
        
        fetchVariantsForProduct(p.maSp);
        window.scrollTo({ top: 0, behavior: "smooth" });
    };

    const resetProductForm = () => {
        setEditingProductId(null);
        setTenSp(""); setDonGia(""); setGiaKhuyenMai(""); setMoTa("");
        setImageFile(null); setVariants([]);
        if (fileInputRef.current) fileInputRef.current.value = "";
    };

    // ==========================================
    // 4. XỬ LÝ NGHIỆP VỤ BIẾN THỂ (CON)
    // ==========================================
    const handleVariantSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!editingProductId) return;
        if (!sku.trim()) return alert("Vui lòng nhập Mã kho (SKU)!");

        try {
            await VariantService.create({
                maSp: editingProductId, maSize, maMau,
                soLuongTon: soLuongTon ? Number(soLuongTon) : 0,
                sku: sku.trim()
            });
            alert("Đã thêm phân loại hàng vào kho!");
            setSku(""); setSoLuongTon("");
            fetchVariantsForProduct(editingProductId);
        } catch (error: any) {
            alert("Lỗi thêm biến thể: " + (error?.response?.data || error.message));
        }
    };

    const handleDeleteVariant = async (id: number) => {
        if (!confirm("Xóa biến thể này?")) return;
        try {
            await VariantService.delete(id);
            if (editingProductId) fetchVariantsForProduct(editingProductId);
        } catch (error: any) {
            alert("Lỗi xóa biến thể: " + (error?.response?.data || error.message));
        }
    };

    // ==========================================
    // GIAO DIỆN
    // ==========================================
    return (
        <div className="max-w-6xl mx-auto p-6 space-y-8">
            <h1 className="text-3xl font-bold text-gray-800">Quản lý Sản Phẩm & Kho</h1>

            {/* PHẦN 1: FORM SẢN PHẨM CHA */}
            <form onSubmit={handleProductSubmit} className={`p-6 rounded-xl shadow-md border-t-4 transition-all ${editingProductId ? 'bg-orange-50 border-orange-500' : 'bg-white border-blue-500'}`}>
                <h2 className={`text-xl font-bold mb-6 flex items-center gap-2 ${editingProductId ? 'text-orange-700' : 'text-blue-700'}`}>
                    {editingProductId ? <Edit /> : <PlusCircle />}
                    {editingProductId ? `Đang Sửa Giày #${editingProductId}` : 'Đăng Bán Giày Mới'}
                </h2>
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div className="space-y-4">
                        <input type="text" value={tenSp} onChange={e => setTenSp(e.target.value)} placeholder="Tên Đôi Giày *" className="w-full px-4 py-2 border rounded focus:ring-2 focus:ring-blue-500" />
                        <div className="grid grid-cols-2 gap-4">
                            <input type="number" value={donGia} onChange={e => setDonGia(e.target.value)} placeholder="Giá Bán (VNĐ) *" className="w-full px-4 py-2 border rounded" />
                            <input type="number" value={giaKhuyenMai} onChange={e => setGiaKhuyenMai(e.target.value)} placeholder="Giá Khuyến Mãi" className="w-full px-4 py-2 border rounded" />
                        </div>
                        <div className="grid grid-cols-2 gap-4">
                            <div>
                                <label className="block text-xs text-gray-500 mb-1">Danh Mục *</label>
                                <select value={maDm} onChange={e => setMaDm(Number(e.target.value))} className="w-full px-4 py-2 border rounded bg-white">
                                    {categories.map(cat => <option key={cat.maDm} value={cat.maDm}>{cat.tenDm}</option>)}
                                </select>
                            </div>
                            <div>
                                <label className="block text-xs text-gray-500 mb-1">Thương Hiệu *</label>
                                <select value={maTh} onChange={e => setMaTh(Number(e.target.value))} className="w-full px-4 py-2 border rounded bg-white">
                                    {brands.length === 0 ? <option value={0}>Chưa có thương hiệu</option> : null}
                                    {brands.map(brand => <option key={brand.maTh} value={brand.maTh}>{brand.tenTh}</option>)}
                                </select>
                            </div>
                        </div>
                    </div>

                    <div className="space-y-4">
                        <input type="file" ref={fileInputRef} onChange={e => setImageFile(e.target.files ? e.target.files[0] : null)} className="w-full px-4 py-2 border rounded bg-white" />
                        <textarea value={moTa} onChange={e => setMoTa(e.target.value)} rows={3} placeholder="Mô tả chi tiết..." className="w-full px-4 py-2 border rounded resize-none" />
                    </div>
                </div>

                <div className="mt-6 flex justify-end gap-3">
                    {editingProductId && (
                        <button type="button" onClick={resetProductForm} className="bg-gray-300 hover:bg-gray-400 px-6 py-2 rounded font-bold flex items-center gap-2">
                            <X size={18}/> Hủy Sửa
                        </button>
                    )}
                    <button type="submit" className={`px-8 py-2 rounded font-bold text-white flex items-center gap-2 ${editingProductId ? 'bg-orange-600' : 'bg-blue-600'}`}>
                        <Upload size={18} /> {editingProductId ? 'Lưu Thông Tin Giày' : 'Tạo Giày Mới'}
                    </button>
                </div>
            </form>

            {/* PHẦN 2: QUẢN LÝ KHO (CHỈ HIỆN KHI ĐANG SỬA SẢN PHẨM) */}
            {editingProductId && (
                <div className="bg-white p-6 rounded-xl shadow-md border border-gray-200 animate-in fade-in slide-in-from-top-4 duration-300">
                    <h2 className="text-xl font-bold text-gray-800 mb-6 flex items-center gap-2 border-b pb-3">
                        <Package className="text-green-600" /> Quản Lý Phân Loại (Kho Hàng)
                    </h2>
                    
                    <form onSubmit={handleVariantSubmit} className="flex flex-wrap gap-4 items-end bg-gray-50 p-4 rounded-lg mb-6 border">
                        <div className="flex-1 min-w-[150px]">
                            <label className="block text-sm font-medium mb-1">Size</label>
                            <select value={maSize} onChange={e => setMaSize(Number(e.target.value))} className="w-full p-2 border rounded">
                                {sizes.map(s => <option key={s.maSize} value={s.maSize}>{s.tenSize}</option>)}
                            </select>
                        </div>
                        <div className="flex-1 min-w-[150px]">
                            <label className="block text-sm font-medium mb-1">Màu Sắc</label>
                            <select value={maMau} onChange={e => setMaMau(Number(e.target.value))} className="w-full p-2 border rounded">
                                {colors.map(c => <option key={c.maMau} value={c.maMau}>{c.tenMau}</option>)}
                            </select>
                        </div>
                        <div className="flex-1 min-w-[150px]">
                            <label className="block text-sm font-medium mb-1">Số Lượng</label>
                            <input type="number" value={soLuongTon} onChange={e => setSoLuongTon(e.target.value)} placeholder="0" className="w-full p-2 border rounded" />
                        </div>
                        <div className="flex-1 min-w-[150px]">
                            <label className="block text-sm font-medium mb-1">Mã Kho (SKU) *</label>
                            <input type="text" value={sku} onChange={e => setSku(e.target.value)} placeholder="VD: NK-AF1-42-BLK" className="w-full p-2 border rounded uppercase" />
                        </div>
                        <button type="submit" className="bg-green-600 hover:bg-green-700 text-white px-6 py-2 rounded font-bold h-[42px] transition-colors">
                            + Thêm Kho
                        </button>
                    </form>

                    <table className="w-full border-collapse bg-white">
                        <thead className="bg-gray-100 text-sm">
                            <tr>
                                <th className="p-3 text-left border">Size</th>
                                <th className="p-3 text-left border">Màu</th>
                                <th className="p-3 text-center border">Mã SKU</th>
                                <th className="p-3 text-right border">Tồn Kho</th>
                                <th className="p-3 text-center border w-24">Xóa</th>
                            </tr>
                        </thead>
                        <tbody>
                            {variants.length === 0 ? (
                                <tr><td colSpan={5} className="p-4 text-center text-gray-500">Giày này chưa có trong kho. Hãy nhập size/màu ở trên.</td></tr>
                            ) : (
                                variants.map(v => (
                                    <tr key={v.maBienThe} className="hover:bg-gray-50">
                                        <td className="p-3 border font-medium text-blue-600">{v.tenSize}</td>
                                        <td className="p-3 border font-medium text-orange-600">{v.tenMau}</td>
                                        <td className="p-3 border text-center text-sm font-mono">{v.sku}</td>
                                        <td className="p-3 border text-right font-bold">{v.soLuongTon}</td>
                                        <td className="p-3 border text-center">
                                            <button onClick={() => handleDeleteVariant(v.maBienThe)} className="text-red-500 hover:bg-red-50 p-1 rounded"><Trash2 size={18}/></button>
                                        </td>
                                    </tr>
                                ))
                            )}
                        </tbody>
                    </table>
                </div>
            )}

            {/* PHẦN 3: BẢNG SẢN PHẨM TỔNG */}
            <div className="bg-white rounded-xl shadow-md overflow-hidden border">
                <table className="w-full text-left">
                    <thead className="bg-gray-800 text-white">
                        <tr>
                            <th className="p-3 w-12 text-center">STT</th>
                            <th className="p-3">Tên Giày</th>
                            <th className="p-3">Danh Mục</th>
                            <th className="p-3">Thương Hiệu</th>
                            <th className="p-3 text-right">Giá</th>
                            <th className="p-3 text-center">Hành Động</th>
                        </tr>
                    </thead>
                    <tbody>
                        {products.map((p, index) => (
                            <tr key={p.maSp} className="border-b hover:bg-gray-50">
                                <td className="p-3 text-center text-gray-500">{index + 1}</td>
                                <td className="p-3 font-bold">{p.tenSp}</td>
                                <td className="p-3 text-blue-600">{p.tenDm || "Chưa có"}</td>
                                <td className="p-3 text-purple-600 font-medium">{p.tenTh || "Chưa có"}</td>
                                <td className="p-3 text-right font-bold text-orange-600">{p.donGia.toLocaleString()} đ</td>
                                <td className="p-3 text-center flex justify-center gap-2">
                                    <button onClick={() => handleEditProduct(p)} className="p-2 text-blue-600 hover:bg-blue-50 rounded"><Edit size={18}/></button>
                                    <button onClick={() => ProductService.delete(p.maSp).then(fetchInitialData)} className="p-2 text-red-500 hover:bg-red-50 rounded"><Trash2 size={18}/></button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
