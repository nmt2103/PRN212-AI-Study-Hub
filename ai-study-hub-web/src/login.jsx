import { Link } from "react-router-dom";

export default function Login() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100">
      <div className="bg-white p-8 rounded-xl shadow-lg max-w-md w-full">
        <h2 className="text-3xl font-bold text-center text-gray-800 mb-6">Đăng Nhập</h2>
        
        <form className="space-y-5">
          <div>
            <label className="block text-gray-700 font-medium mb-1">Email</label>
            <input 
              type="email" 
              placeholder="Nhập email của bạn..." 
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          <div>
            <label className="block text-gray-700 font-medium mb-1">Mật khẩu</label>
            <input 
              type="password" 
              placeholder="Nhập mật khẩu..." 
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            {/* Dòng chữ Forgot Password mờ mờ */}
            <div className="text-right mt-1">
              <a href="#" className="text-sm text-gray-400 hover:text-blue-500 transition">
                Quên mật khẩu? (Forgot password)
              </a>
            </div>
          </div>

          <button 
            type="submit" 
            className="w-full bg-blue-600 text-white py-2 rounded-lg font-bold hover:bg-blue-700 transition"
          >
            Đăng nhập
          </button>
        </form>

        <p className="text-center text-gray-600 mt-6">
          Chưa có tài khoản? <Link to="/register" className="text-blue-600 font-semibold hover:underline">Đăng ký ngay</Link>
        </p>
      </div>
    </div>
  );
}
