export interface Product {
    maSp: number;
    maDm?: number | null;
    maTh?: number | null;
    tenSp: string;
    donGia: number;
    giaKhuyenMai?: number | null;
    moTa?: string | null;
    tenDm?: string | null;
    tenTh?: string | null;
}

export interface ProductCreateRequest {
    tenSp: string;
    donGia: number;
    giaKhuyenMai?: number | null;
    moTa?: string | null;
    maDm: number;
    maTh: number; 
}
