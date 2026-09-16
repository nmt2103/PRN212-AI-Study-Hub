import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Welcome from "./welcome";
import Login from "./login";
import Register from "./register";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Welcome />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
      </Routes>
    </Router>
  );
}

export default App;
