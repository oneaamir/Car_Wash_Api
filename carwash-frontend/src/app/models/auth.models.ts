// Auth Models - TypeScript Interfaces
// Interface = ek blueprint jo batata hai ki object mein kya kya fields honge
// Backend ke DTOs se match karta hai

// Login ke liye backend ko yeh data bhejenge
export interface LoginRequest {
  email: string;
  password: string;
}

// Register ke liye backend ko yeh data bhejenge
export interface RegisterRequest {
  fullName: string;
  email: string;
  phone: string;
  password: string;
  role: string;
}

// Login/Register ke baad backend se yeh data milega
export interface AuthResponse {
  userId: number;
  fullName: string;
  email: string;
  role: string;    // "Customer", "Admin", ya "Washer"
  token: string;   // JWT token - yeh har API call mein bhejenge
  message: string;
}

// Profile page ke liye
export interface ProfileResponse {
  userId: number;
  fullName: string;
  email: string;
  role: string;
  phone: string;
  message: string;
}
