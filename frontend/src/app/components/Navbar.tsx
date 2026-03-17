"use client";

import React from "react";
import { Menu } from "lucide-react";

interface NavbarProps {
  toggleSidebar: () => void;
}

export default function Navbar({ toggleSidebar }: NavbarProps) {
  return (
    <header className="h-16 bg-white shadow-sm flex items-center justify-between px-6 z-10 relative">
      <button
        onClick={toggleSidebar}
        className="p-2 rounded-md hover:bg-gray-100 text-gray-600 transition-colors"
      >
        <Menu size={24} />
      </button>

      {/* User Menu */}
      <div className="flex items-center gap-4">
        <div className="flex items-center gap-2 cursor-pointer hover:bg-gray-50 p-2 rounded-md transition-colors border border-transparent hover:border-gray-200">
          <div className="w-8 h-8 rounded-full bg-blue-600 flex items-center justify-center text-white font-bold">
            A
          </div>
          <span className="font-medium text-sm">Admin</span>
        </div>
      </div>
    </header>
  );
}