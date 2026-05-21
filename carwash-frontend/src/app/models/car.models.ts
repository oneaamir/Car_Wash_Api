export interface Car {
  id: number;
  userId: number;
  carNumber: string;
  brand: string;
  model: string;
  carType: string;
  imageUrl: string;
  isActive: boolean;
  message: string;
}

export interface CreateCarRequest {
  carNumber: string;
  brand: string;
  model: string;
  carType: string;
  imageUrl: string | null;
}

export interface UpdateCarRequest {
  carNumber: string;
  brand: string;
  model: string;
  carType: string;
  imageUrl: string | null;
}
