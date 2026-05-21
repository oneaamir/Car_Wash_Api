export interface AdminBooking {
  id: number;
  userId: number;
  carId: number;
  servicePlanId: number;
  promoCodeId: number | null;
  assignedWasherId: number | null;
  customerName: string;
  customerEmail: string;
  carNumber: string;
  servicePlanName: string;
  promoCode: string;
  assignedWasherName: string;
  assignedWasherEmail: string;
  addOnNames: string[];
  bookingType: string;
  bookingDate: string;
  address: string;
  status: string;
  totalAmount: number;
  message: string;
}

export interface AdminUser {
  id: number;
  fullName: string;
  email: string;
  phone: string;
  role: string;
  isActive: boolean;
  message: string;
}

export interface AssignWasherRequest {
  washerId: number;
}

export interface UpdateBookingStatusRequest {
  status: string;
}

export interface UpdatePaymentStatusRequest {
  paymentStatus: string;
}
