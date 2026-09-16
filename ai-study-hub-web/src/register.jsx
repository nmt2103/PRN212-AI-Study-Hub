import { Link } from "react-router-dom";

export default function Register() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white p-8 rounded-xl shadow-lg max-w-md w-full my-8">
        <h2 className="text-3xl font-bold text-center text-gray-800 mb-6">Đăng Ký Tài Khoản</h2>
        
        <form className="space-y-4">
          <div>
            <label className="block text-gray-700 font-medium mb-1">Họ và Tên</label>
            <input 
              type="text" 
              placeholder="VD: Nguyễn Văn A..." 
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div>
            <label className="block text-gray-700 font-medium mb-1">Email</label>
            <input 
              type="email" 
              placeholder="Nhập email..." 
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div>
            <label className="block text-gray-700 font-medium mb-1">Mật khẩu</label>
            <input 
              type="password" 
              placeholder="Tạo mật khẩu..." 
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div>
            <label className="block text-gray-700 font-medium mb-1">Xác nhận Mật khẩu</label>
            <input 
              type="password" 
              placeholder="Nhập lại mật khẩu..." 
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <button 
            type="submit" 
            className="w-full bg-blue-600 text-white py-2 rounded-lg font-bold hover:bg-blue-700 transition mt-4"
          >
            Đăng ký
          </button>
        </form>

        <p className="text-center text-gray-600 mt-6">
          Đã có tài khoản? <Link to="/login" className="text-blue-600 font-semibold hover:underline">Đăng nhập</Link>
        </p>
      </div>
    </div>
  );
}
