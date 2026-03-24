export interface Category {
    id: number;
    tenDanhMuc: string;
    moTa?: string;
    trangThai: boolean;
    soSanPham: number;
}

export interface CategoryRequest {
    tenDanhMuc: string;
    moTa?: string;
    trangThai: boolean;
}