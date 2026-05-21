export interface ServicePlan {
  id: number;
  name: string;
  description: string;
  price: number;
  isActive: boolean;
  message: string;
}

export interface AddOn {
  id: number;
  name: string;
  price: number;
  isActive: boolean;
  message: string;
}
