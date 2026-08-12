import { Component, OnInit } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { DatePipe } from '@angular/common';

import {
  FormBuilder,
  FormsModule,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { DatePickerModule } from 'primeng/datepicker';
import { PaginatorModule } from 'primeng/paginator';
import { ProgressBarModule } from 'primeng/progressbar';
import { MenuModule } from 'primeng/menu';

import {
  ConfirmationService,
  MessageService,
  MenuItem
} from 'primeng/api';

import { TodoService } from './core/services/todo.service';
import { CategoryService } from './core/services/category.service';

import { Todo } from './core/models/todo.model';
import { Category } from './core/models/category.model';


@Component({
  selector: 'app-root',

  imports: [
    DatePipe,
    ButtonModule,
    CardModule,
    TagModule,
    FormsModule,
    ReactiveFormsModule,
    CheckboxModule,
    ConfirmDialogModule,
    ToastModule,
    DialogModule,
    InputTextModule,
    TextareaModule,
    SelectModule,
    DatePickerModule,
    PaginatorModule,
    ProgressBarModule,
    MenuModule
  ],

  providers: [
    ConfirmationService,
    MessageService
  ],

  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {

  todos: Todo[] = [];

  categories: Category[] = [];


  // Sayaçlar
  totalCount = 0;

  allTodoCount = 0;

  activeCount = 0;

  completedCount = 0;

  completionRate = 0;


  // Loading
  isLoading = false;

  isSaving = false;

  isCategorySaving = false;


  // Todo Dialog
  todoDialogVisible = false;

  isEditMode = false;

  editingTodoId: number | null = null;


  // Kategori Dialog
  categoryManagerVisible = false;

  isCategoryEditMode = false;

  editingCategoryId: number | null = null;


  // Bugünün tarihi
  today = new Date();


  // Sayfalama
  pageNumber = 1;

  pageSize = 5;

  first = 0;


  // Filtreler
  searchText = '';

  // Aramanın görev başlığında mı yoksa açıklamada mı yapılacağını tutar.
  selectedSearchField = 'title';

  selectedStatus: string | null = null;

  selectedPriority: number | null = null;

  selectedCategoryId: number | null = null;

  selectedSortBy = 'createdAt';

  selectedSortDirection = 'desc';


  // Arama alanı seçenekleri
  searchFieldOptions = [
    {
      label: 'Başlık',
      value: 'title'
    },
    {
      label: 'Açıklama',
      value: 'description'
    }
  ];


  statusOptions = [
    {
      label: 'Aktif',
      value: 'active'
    },
    {
      label: 'Tamamlandı',
      value: 'completed'
    },
    {
      label: 'Süresi Geçmiş',
      value: 'overdue'
    }
  ];


  priorityOptions = [
    {
      label: 'Düşük',
      value: 1
    },
    {
      label: 'Orta',
      value: 2
    },
    {
      label: 'Yüksek',
      value: 3
    },
    {
      label: 'Kritik',
      value: 4
    }
  ];


  categoryFilterOptions: {
    label: string;
    value: number;
  }[] = [];


  sortByOptions = [
    {
      label: 'Oluşturulma Tarihi',
      value: 'createdAt'
    },
    {
      label: 'Son Teslim Tarihi',
      value: 'dueDate'
    }
  ];


  sortDirectionOptions = [
    {
      label: 'Yeniden Eskiye',
      value: 'desc'
    },
    {
      label: 'Eskiden Yeniye',
      value: 'asc'
    }
  ];


  // Görev kartındaki üç nokta işlem menüsünün seçenekleri
  todoMenuItems: MenuItem[] = [];


  todoForm;

  categoryForm;


  constructor(
    private todoService: TodoService,
    private categoryService: CategoryService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private formBuilder: FormBuilder
  ) {

    // Bugünün saatini sıfırlıyoruz.
    // Böylece minDate sadece güne göre çalışır.
    this.today.setHours(
      0,
      0,
      0,
      0
    );


    // TODO FORM
    this.todoForm =
      this.formBuilder.group({

        title: [
          '',
          [
            Validators.required,
            Validators.minLength(3),
            Validators.maxLength(150)
          ]
        ],

        description: [
          '',
          [
            Validators.maxLength(1000)
          ]
        ],

        priority: [
          2,
          Validators.required
        ],

        dueDate: [
          null as Date | null
        ],

        categoryId: [
          null as number | null
        ]

      });


    // CATEGORY FORM
    this.categoryForm =
      this.formBuilder.group({

        name: [
          '',
          [
            Validators.required,
            Validators.maxLength(100)
          ]
        ],

        color: [
          '#3B82F6',
          [
            Validators.maxLength(20)
          ]
        ]

      });

  }


  ngOnInit(): void {

    this.loadCategories();

    this.loadTodos();

    this.loadStats();

  }


  // =================================================
  // TARİH YARDIMCI METOTLARI
  // =================================================


  // DatePicker'dan gelen Date'i:
  //
  // 20 Ağustos 2026
  //
  // şeklinden:
  //
  // 2026-08-20
  //
  // şekline çevirir.
  formatDateForApi(
    date: Date | null
  ): string | null {

    if (!date) {
      return null;
    }


    const year =
      date.getFullYear();


    const month =
      String(
        date.getMonth() + 1
      ).padStart(
        2,
        '0'
      );


    const day =
      String(
        date.getDate()
      ).padStart(
        2,
        '0'
      );


    return `${year}-${month}-${day}`;
  }


  // Backend'den gelen tarihi
  // PrimeNG DatePicker'ın istediği
  // Date nesnesine çevirir.
  parseApiDate(
    value: string | undefined
  ): Date | null {

    if (!value) {
      return null;
    }


    // Backend:
    // 2026-08-20T00:00:00
    //
    // İlk 10 karakter:
    // 2026-08-20

    const datePart =
      value.substring(
        0,
        10
      );


    const parts =
      datePart.split('-');


    if (parts.length !== 3) {
      return null;
    }


    const year =
      Number(parts[0]);

    const month =
      Number(parts[1]);

    const day =
      Number(parts[2]);


    return new Date(
      year,
      month - 1,
      day
    );

  }


  // =================================================
  // KATEGORİLER
  // =================================================

  loadCategories(): void {

    this.categoryService
      .getCategories()
      .subscribe({

        next: (response) => {

          this.categories =
            response.data ?? [];


          this.categoryFilterOptions = [

            {
              label: 'Kategorisiz',
              value: 0
            },

            ...this.categories.map(
              category => ({
                label: category.name,
                value: category.id
              })
            )

          ];

        },

        error: (error) => {

          console.error(
            'Kategoriler alınamadı:',
            error
          );


          this.messageService.add({
            severity: 'error',
            summary: 'Hata',
            detail: 'Kategoriler alınamadı.'
          });

        }

      });

  }


  // =================================================
  // TODO'LARI GETİR
  // =================================================

  loadTodos(): void {

    this.isLoading = true;


    let params =
      new HttpParams()
        .set(
          'pageNumber',
          this.pageNumber.toString()
        )
        .set(
          'pageSize',
          this.pageSize.toString()
        )
        .set(
          'sortBy',
          this.selectedSortBy
        )
        .set(
          'sortDirection',
          this.selectedSortDirection
        );


    if (this.searchText.trim()) {

      params = params
        .set(
          'search',
          this.searchText.trim()
        )
        .set(
          'searchField',
          this.selectedSearchField
        );

    }


    if (this.selectedStatus) {

      params = params.set(
        'status',
        this.selectedStatus
      );

    }


    if (this.selectedPriority !== null) {

      params = params.set(
        'priority',
        this.selectedPriority.toString()
      );

    }


    if (this.selectedCategoryId !== null) {

      params = params.set(
        'categoryId',
        this.selectedCategoryId.toString()
      );

    }


    this.todoService
      .getTodos(params)
      .subscribe({

        next: (response) => {

          this.todos =
            response.data?.items ?? [];

          this.totalCount =
            response.data?.totalCount ?? 0;

          this.isLoading = false;

        },

        error: (error) => {

          this.isLoading = false;


          console.error(
            'Todo listesi alınamadı:',
            error
          );


          this.messageService.add({
            severity: 'error',
            summary: 'Hata',
            detail: 'Görev listesi alınamadı.'
          });

        }

      });


  }


  // =================================================
  // SAYAÇLARI GETİR
  // =================================================

  loadStats(): void {

    const allParams =
      new HttpParams()
        .set(
          'pageSize',
          '1'
        );


    this.todoService
      .getTodos(allParams)
      .subscribe({

        next: (response) => {

          this.allTodoCount =
            response.data?.totalCount ?? 0;

          this.calculateCompletionRate();

        }

      });


    const activeParams =
      new HttpParams()
        .set(
          'status',
          'active'
        )
        .set(
          'pageSize',
          '1'
        );


    this.todoService
      .getTodos(activeParams)
      .subscribe({

        next: (response) => {

          this.activeCount =
            response.data?.totalCount ?? 0;

        }

      });


    const completedParams =
      new HttpParams()
        .set(
          'status',
          'completed'
        )
        .set(
          'pageSize',
          '1'
        );


    this.todoService
      .getTodos(completedParams)
      .subscribe({

        next: (response) => {

          this.completedCount =
            response.data?.totalCount ?? 0;

          this.calculateCompletionRate();

        }

      });

  }


  calculateCompletionRate(): void {

    if (this.allTodoCount === 0) {

      this.completionRate = 0;

      return;

    }


    this.completionRate =
      Math.round(
        (
          this.completedCount /
          this.allTodoCount
        ) * 100
      );

  }


  // =================================================
  // FİLTRE
  // =================================================

  applyFilters(): void {

    this.pageNumber = 1;

    this.first = 0;

    this.loadTodos();

  }


  clearFilters(): void {

    this.searchText = '';

    this.selectedSearchField = 'title';

    this.selectedStatus = null;

    this.selectedPriority = null;

    this.selectedCategoryId = null;

    this.selectedSortBy = 'createdAt';

    this.selectedSortDirection = 'desc';

    this.pageNumber = 1;

    this.pageSize = 5;

    this.first = 0;

    this.loadTodos();

  }


  onPageChange(
    event: any
  ): void {

    this.first =
      event.first ?? 0;

    this.pageSize =
      event.rows ?? 5;

    this.pageNumber =
      (event.page ?? 0) + 1;

    this.loadTodos();

  }


  // =================================================
  // YENİ TODO
  // =================================================

  openTodoDialog(): void {

    this.isEditMode = false;

    this.editingTodoId = null;


    this.todoForm.reset({

      title: '',

      description: '',

      priority: 2,

      dueDate: null,

      categoryId: null

    });


    this.todoDialogVisible = true;

  }


  // =================================================
  // TODO DÜZENLE
  // =================================================

  openEditDialog(
    todo: Todo
  ): void {

    this.isEditMode = true;

    this.editingTodoId =
      todo.id;


    this.todoForm.reset({

      title:
        todo.title,

      description:
        todo.description ?? '',

      priority:
        todo.priority,

      // BURASI DEĞİŞTİ
      dueDate:
        this.parseApiDate(
          todo.dueDate
        ),

      categoryId:
        todo.categoryId ?? null

    });


    this.todoDialogVisible = true;

  }


  // =================================================
  // TODO KAYDET / GÜNCELLE
  // =================================================

  saveTodo(): void {

    if (this.todoForm.invalid) {

      this.todoForm.markAllAsTouched();


      this.messageService.add({
        severity: 'warn',
        summary: 'Form Hatası',
        detail: 'Form alanlarını kontrol ediniz.'
      });


      return;

    }


    const formValue =
      this.todoForm.getRawValue();


    const request = {

      title:
        formValue.title ?? '',

      description:
        formValue.description || null,

      priority:
        formValue.priority ?? 2,

      // BURASI DEĞİŞTİ
      //
      // Date nesnesini direkt göndermiyoruz.
      dueDate:
        this.formatDateForApi(
          formValue.dueDate
        ),

      categoryId:
        formValue.categoryId

    };


    this.isSaving = true;


    // -------------------------
    // GÜNCELLE
    // -------------------------

    if (
      this.isEditMode &&
      this.editingTodoId !== null
    ) {

      this.todoService
        .updateTodo(
          this.editingTodoId,
          request
        )
        .subscribe({

          next: () => {

            this.isSaving = false;


            this.messageService.add({
              severity: 'success',
              summary: 'Başarılı',
              detail: 'Görev başarıyla güncellendi.'
            });


            this.todoDialogVisible =
              false;

            this.isEditMode =
              false;

            this.editingTodoId =
              null;


            this.loadTodos();

          },

          error: (error) => {

            this.isSaving = false;


            console.error(
              'Görev güncellenemedi:',
              error
            );


            this.messageService.add({
              severity: 'error',
              summary: 'Hata',
              detail: 'Görev güncellenemedi.'
            });

          }

        });


      return;

    }


    // -------------------------
    // YENİ TODO
    // -------------------------

    this.todoService
      .createTodo(
        request
      )
      .subscribe({

        next: () => {

          this.isSaving = false;


          this.messageService.add({
            severity: 'success',
            summary: 'Başarılı',
            detail: 'Görev başarıyla oluşturuldu.'
          });


          this.todoDialogVisible =
            false;


          this.loadTodos();

          this.loadStats();

        },

        error: (error) => {

          this.isSaving = false;


          console.error(
            'Görev oluşturulamadı:',
            error
          );


          this.messageService.add({
            severity: 'error',
            summary: 'Hata',
            detail: 'Görev oluşturulamadı.'
          });

        }

      });

  }


  // =================================================
  // TODO İŞLEM MENÜSÜ
  // =================================================

  openTodoMenu(
    event: Event,
    todo: Todo,
    menu: any
  ): void {

    // Tıklanan Todo'ya özel menü seçeneklerini hazırlıyoruz.
    this.todoMenuItems = [
      {
        label: 'Düzenle',
        icon: 'pi pi-pencil',
        command: () => {
          this.openEditDialog(todo);
        }
      },
      {
        label: todo.isCompleted
          ? 'Tekrar Aktif Yap'
          : 'Tamamlandı Yap',
        icon: todo.isCompleted
          ? 'pi pi-refresh'
          : 'pi pi-check',
        command: () => {
          this.changeTodoStatus(
            todo,
            !todo.isCompleted
          );
        }
      },
      {
        separator: true
      },
      {
        label: 'Sil',
        icon: 'pi pi-trash',
        command: () => {
          this.confirmDelete(todo);
        }
      }
    ];

    // PrimeNG popup menüsünü tıklanan butonun yanında açar.
    menu.toggle(event);
  }



  // =================================================
  // TODO DURUM
  // =================================================

  changeTodoStatus(
    todo: Todo,
    isCompleted: boolean
  ): void {

    this.todoService
      .changeStatus(
        todo.id,
        isCompleted
      )
      .subscribe({

        next: () => {

          this.messageService.add({

            severity: 'success',

            summary: 'Başarılı',

            detail:
              isCompleted
                ? 'Görev tamamlandı.'
                : 'Görev tekrar aktif duruma alındı.'

          });


          this.loadTodos();

          this.loadStats();

        },

        error: (error) => {

          console.error(
            'Görev durumu değiştirilemedi:',
            error
          );


          this.messageService.add({
            severity: 'error',
            summary: 'Hata',
            detail: 'Görev durumu değiştirilemedi.'
          });

        }

      });

  }


  // =================================================
  // TODO SİL
  // =================================================

  confirmDelete(
    todo: Todo
  ): void {

    this.confirmationService.confirm({

      header:
        'Görevi Sil',

      message:
        `"${todo.title}" görevini silmek istediğine emin misin?`,

      icon:
        'pi pi-exclamation-triangle',

      acceptLabel:
        'Evet, Sil',

      rejectLabel:
        'İptal',

      accept: () => {

        this.deleteTodo(
          todo.id
        );

      }

    });

  }


  deleteTodo(
    id: number
  ): void {

    this.todoService
      .deleteTodo(
        id
      )
      .subscribe({

        next: () => {

          this.messageService.add({
            severity: 'success',
            summary: 'Başarılı',
            detail: 'Görev başarıyla silindi.'
          });


          this.loadTodos();

          this.loadStats();

        },

        error: (error) => {

          console.error(
            'Görev silinemedi:',
            error
          );


          this.messageService.add({
            severity: 'error',
            summary: 'Hata',
            detail: 'Görev silinemedi.'
          });

        }

      });

  }


  // =================================================
  // KATEGORİ YÖNETİMİ
  // =================================================

  openCategoryManager(): void {

    this.resetCategoryForm();

    this.categoryManagerVisible =
      true;

  }


  resetCategoryForm(): void {

    this.isCategoryEditMode =
      false;

    this.editingCategoryId =
      null;


    this.categoryForm.reset({

      name: '',

      color: '#3B82F6'

    });

  }


  editCategory(
    category: Category
  ): void {

    this.isCategoryEditMode =
      true;

    this.editingCategoryId =
      category.id;


    this.categoryForm.reset({

      name:
        category.name,

      color:
        category.color ??
        '#3B82F6'

    });

  }


  saveCategory(): void {

    if (this.categoryForm.invalid) {

      this.categoryForm.markAllAsTouched();


      this.messageService.add({
        severity: 'warn',
        summary: 'Form Hatası',
        detail: 'Kategori alanlarını kontrol ediniz.'
      });


      return;

    }


    const formValue =
      this.categoryForm.getRawValue();


    const request = {

      name:
        formValue.name ?? '',

      color:
        formValue.color || null

    };


    this.isCategorySaving =
      true;


    if (
      this.isCategoryEditMode &&
      this.editingCategoryId !== null
    ) {

      this.categoryService
        .updateCategory(
          this.editingCategoryId,
          request
        )
        .subscribe({

          next: () => {

            this.isCategorySaving =
              false;


            this.messageService.add({
              severity: 'success',
              summary: 'Başarılı',
              detail: 'Kategori başarıyla güncellendi.'
            });


            this.resetCategoryForm();

            this.loadCategories();

            this.loadTodos();

          },

          error: (error) => {

            this.isCategorySaving =
              false;


            console.error(
              'Kategori güncellenemedi:',
              error
            );


            this.messageService.add({
              severity: 'error',
              summary: 'Hata',
              detail: 'Kategori güncellenemedi.'
            });

          }

        });


      return;

    }


    this.categoryService
      .createCategory(
        request
      )
      .subscribe({

        next: () => {

          this.isCategorySaving =
            false;


          this.messageService.add({
            severity: 'success',
            summary: 'Başarılı',
            detail: 'Kategori başarıyla oluşturuldu.'
          });


          this.resetCategoryForm();

          this.loadCategories();

        },

        error: (error) => {

          this.isCategorySaving =
            false;


          console.error(
            'Kategori oluşturulamadı:',
            error
          );


          this.messageService.add({
            severity: 'error',
            summary: 'Hata',
            detail: 'Kategori oluşturulamadı.'
          });

        }

      });

  }


  confirmDeleteCategory(
    category: Category
  ): void {

    this.confirmationService.confirm({

      header:
        'Kategoriyi Sil',

      message:
        `"${category.name}" kategorisini silmek istediğine emin misin?`,

      icon:
        'pi pi-exclamation-triangle',

      acceptLabel:
        'Evet, Sil',

      rejectLabel:
        'İptal',

      accept: () => {

        this.deleteCategory(
          category.id
        );

      }

    });

  }


  deleteCategory(
    id: number
  ): void {

    this.categoryService
      .deleteCategory(
        id
      )
      .subscribe({

        next: () => {

          this.messageService.add({
            severity: 'success',
            summary: 'Başarılı',
            detail: 'Kategori başarıyla silindi.'
          });


          if (
            this.selectedCategoryId === id
          ) {

            this.selectedCategoryId =
              null;

          }


          this.resetCategoryForm();

          this.loadCategories();

          this.loadTodos();

        },

        error: (error) => {

          console.error(
            'Kategori silinemedi:',
            error
          );


          this.messageService.add({
            severity: 'error',
            summary: 'Hata',
            detail: 'Kategori silinemedi.'
          });

        }

      });

  }

}