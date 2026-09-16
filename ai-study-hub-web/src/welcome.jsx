import { Link } from "react-router-dom";

export default function Welcome() {
  return (
    <div className="flex h-screen bg-blue-50">
      {/* Cột trái: Ảnh minh họa */}
      <div className="hidden lg:flex w-1/2 items-center justify-center bg-blue-100">
        {/* Bạn có thể thay src bằng link ảnh thật của bạn */}
        <img 
          src="https://images.unsplash.com/photo-1522202176988-66273c2fd55f?ixlib=rb-4.0.3&auto=format&fit=crop&w=800&q=80" 
          alt="Study Illustration" 
          className="object-cover h-full w-full opacity-90"
        />
      </div>
 <div className="w-full lg:w-1/2 flex flex-col justify-center items-center p-12 bg-white">
        <div className="max-w-md w-full">
          <h1 className="text-4xl font-bold text-gray-900 mb-2">AI Study Hub 🚀</h1>
          <p className="text-gray-600 mb-10 text-lg">
            Nền tảng quản lý tài liệu học tập thông minh. Đơn giản hóa hành trình học tập của bạn ngay hôm nay!
          </p>

          <div className="flex flex-col gap-4">
            <Link 
              to="/login" 
              className="w-full bg-blue-600 text-white text-center py-3 rounded-lg font-semibold hover:bg-blue-700 transition"
            >
              Đăng nhập (Login)
            </Link>
            <Link 
              to="/register" 
              className="w-full bg-white text-blue-600 border-2 border-blue-600 text-center py-3 rounded-lg font-semibold hover:bg-blue-50 transition"
            >
              Đăng ký tài khoản mới
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}
