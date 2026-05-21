export interface Payment {
  id: number;
  bookingId: number;
  amount: number;
  paymentStatus: string;
  transactionRef: string;
  paymentMethod: string;
  message: string;
}

export interface Receipt {
  id: number;
  bookingId: number;
  paymentId: number;
  receiptNumber: string;
  generatedAt: string;
  afterWashImageUrl: string;
  message: string;
}

export interface CreatePaymentRequest {
  bookingId: number;
  paymentMethod: string;
  transactionRef: string;
}
