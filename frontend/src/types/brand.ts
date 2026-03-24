export interface Brand {
    id: number;
    tenThuongHieu: string;
    logoUrl?: string; // Thêm trường Logo
    moTa?: string;
    trangThai: boolean;
    soSanPham: number;
}

export interface BrandRequest {
    tenThuongHieu: string;
    logoUrl?: string;
    moTa?: string;
    trangThai: boolean;
}