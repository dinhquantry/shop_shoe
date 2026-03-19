// File: types/brand.ts

export interface Brand {
    maTh: number;
    tenTh: string;
    moTa?: string | null;
    logo?: string | null; // C# sẽ trả về chuỗi dạng "/images/brands/abc.jpg"
}

export interface BrandCreateRequest {
    tenTh: string;
    moTa?: string | null;
}