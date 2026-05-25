export interface Booking {
  id: number;
  userId: number;
  carId: number;
  servicePlanId: number;
  promoCodeId: number | null;
  assignedWasherId: number | null;
  carNumber: string;
  carBrand: string;
  carModel: string;
  servicePlanName: string;
  servicePlanDescription: string;
  promoCode: string;
  assignedWasherName: string;
  assignedWasherEmail: string;
  addOnNames: string[];
  bookingType: string;
  bookingDate: string;
  address: string;
  notes: string;
  status: string;
  totalAmount: number;
  customerName: string;
  customerPhone: string;
  createdAt: string;
  message: string;
}

export interface CreateBookingRequest {
  carId: number;
  servicePlanId: number;
  addOnIds: number[];
  bookingType: string;
  bookingDate: string;
  address: string;
  promoCode: string;
  notes: string;
}
