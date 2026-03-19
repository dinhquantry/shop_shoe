export interface Variant {
    maBienThe: number;
    maSp: number;
    tenSp: string;
    maSize: number;
    tenSize: string;
    maMau: number;
    tenMau: string;
    soLuongTon: number | null;
    sku: string | null;
    trangThai: number | null;
}

export interface VariantCreateRequest {
    maSp: number;
    maSize: number;
    maMau: number;
    soLuongTon: number | null;
    sku: string | null;
}