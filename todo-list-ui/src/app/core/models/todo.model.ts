export interface Todo {
  id: number;   // Buradaki Todo interface, backend'deki TodoResponseDtodan Angular'a gelen Todo'nun şeklini tarif ediyor.
  title: string;
  description?: string;
  priority: number;
  dueDate?: string;
  isCompleted: boolean;
  completedAt?: string;
  categoryId?: number;
  categoryName?: string;
  categoryColor?: string;
  createdAt: string;
  updatedAt?: string;
}