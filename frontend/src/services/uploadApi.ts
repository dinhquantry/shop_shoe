import axiosClient from "./axiosClient";

export interface HinhAnhSanPham {
  id: number;
  maSanPham: number;
  imageUrl: string;
  isMain: boolean;
  thuTu: number;
}

export interface UploadAnhSanPhamPayload {
  maSanPham: number;
  files: File[];
  thuTuBatDau?: number;
  anhChinhIndex?: number;
}

export const uploadAnhSanPham = async ({
  maSanPham,
  files,
  thuTuBatDau,
  anhChinhIndex,
}: UploadAnhSanPhamPayload) => {
  const formData = new FormData();
  formData.append("MaSanPham", String(maSanPham));

  files.forEach((file) => {
    formData.append("Files", file);
  });

  if (typeof thuTuBatDau === "number") {
    formData.append("ThuTuBatDau", String(thuTuBatDau));
  }

  if (typeof anhChinhIndex === "number") {
    formData.append("AnhChinhIndex", String(anhChinhIndex));
  }

  return axiosClient.post<HinhAnhSanPham[]>("/Uploads/san-pham-anh", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });
};
