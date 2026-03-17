"use client";

import React from "react";
import { 
  DollarSign, 
  ShoppingBag, 
  Package, 
  Users, 
  TrendingUp, 
  Clock 
} from "lucide-react";

export default function AdminDashboard() {
  // Dữ liệu giả lập (Mock data) cho các thẻ thống kê
  const stats = [
    {
      title: "Tổng doanh thu",
      value: "124,500,000 ₫",
      trend: "+12.5%",
      isPositive: true,
      icon: <DollarSign size={24} className="text-white" />,
      color: "bg-green-500",
    },
    {
      title: "Đơn hàng mới",
      value: "45",
      trend: "+5.2%",
      isPositive: true,
      icon: <ShoppingBag size={24} className="text-white" />,
      color: "bg-blue-500",
    },
    {
      title: "Sản phẩm trong kho",
      value: "1,204",
      trend: "-2.4%",
      isPositive: false,
      icon: <Package size={24} className="text-white" />,
      color: "bg-purple-500",
    },
    {
      title: "Khách hàng",
      value: "892",
      trend: "+18.1%",
      isPositive: true,
      icon: <Users size={24} className="text-white" />,
      color: "bg-orange-500",
    },
  ];

  // Dữ liệu giả lập cho bảng Đơn hàng gần đây
  const recentOrders = [
    { id: "#ORD-001", customer: "Nguyễn Văn A", product: "Nike Air Force 1", total: "2,500,000 ₫", status: "Đang giao", time: "2 giờ trước" },
    { id: "#ORD-002", customer: "Trần Thị B", product: "Adidas Ultraboost", total: "3,200,000 ₫", status: "Đã giao", time: "5 giờ trước" },
    { id: "#ORD-003", customer: "Lê Văn C", product: "Puma Suede", total: "1,800,000 ₫", status: "Chờ xác nhận", time: "1 ngày trước" },
    { id: "#ORD-004", customer: "Phạm D", product: "Vans Old Skool", total: "1,200,000 ₫", status: "Đã giao", time: "1 ngày trước" },
  ];

  return (
    <div className="space-y-6">
      {/* Tiêu đề trang */}
      <div>
        <h1 className="text-2xl font-bold text-gray-800">Tổng quan hệ thống</h1>
        <p className="text-gray-500 mt-1">Theo dõi tình hình kinh doanh cửa hàng giày của bạn.</p>
      </div>

      {/* Grid 4 Thẻ thống kê */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {stats.map((stat, index) => (
          <div key={index} className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 flex items-center gap-4 transition-transform hover:-translate-y-1 duration-300">
            {/* Icon */}
            <div className={`w-14 h-14 rounded-full flex items-center justify-center ${stat.color} shadow-inner`}>
              {stat.icon}
            </div>
            
            {/* Thông tin */}
            <div>
              <p className="text-sm font-medium text-gray-500">{stat.title}</p>
              <h3 className="text-2xl font-bold text-gray-800 mt-1">{stat.value}</h3>
              <div className="flex items-center mt-1">
                <TrendingUp size={14} className={stat.isPositive ? "text-green-500 mr-1" : "text-red-500 mr-1 rotate-180"} />
                <span className={`text-xs font-medium ${stat.isPositive ? "text-green-500" : "text-red-500"}`}>
                  {stat.trend}
                </span>
                <span className="text-xs text-gray-400 ml-1">so với tháng trước</span>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Khu vực Bảng Đơn hàng gần đây */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        <div className="p-6 border-b border-gray-100 flex justify-between items-center">
          <h2 className="text-lg font-bold text-gray-800">Đơn hàng gần đây</h2>
          <button className="text-sm text-blue-600 font-medium hover:text-blue-700">Xem tất cả</button>
        </div>
        
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-gray-50 text-gray-500 text-sm">
                <th className="py-3 px-6 font-medium">Mã ĐH</th>
                <th className="py-3 px-6 font-medium">Khách hàng</th>
                <th className="py-3 px-6 font-medium">Sản phẩm</th>
                <th className="py-3 px-6 font-medium">Tổng tiền</th>
                <th className="py-3 px-6 font-medium">Trạng thái</th>
                <th className="py-3 px-6 font-medium text-right">Thời gian</th>
              </tr>
            </thead>
            <tbody className="text-sm">
              {recentOrders.map((order, index) => (
                <tr key={index} className="border-b border-gray-50 hover:bg-gray-50 transition-colors">
                  <td className="py-4 px-6 font-medium text-blue-600">{order.id}</td>
                  <td className="py-4 px-6 text-gray-800">{order.customer}</td>
                  <td className="py-4 px-6 text-gray-500">{order.product}</td>
                  <td className="py-4 px-6 font-medium text-gray-800">{order.total}</td>
                  <td className="py-4 px-6">
                    <span className={`px-3 py-1 text-xs font-medium rounded-full ${
                      order.status === 'Đã giao' ? 'bg-green-100 text-green-700' :
                      order.status === 'Đang giao' ? 'bg-blue-100 text-blue-700' :
                      'bg-orange-100 text-orange-700'
                    }`}>
                      {order.status}
                    </span>
                  </td>
                  <td className="py-4 px-6 text-right text-gray-400 flex items-center justify-end gap-1">
                    <Clock size={14} />
                    {order.time}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}